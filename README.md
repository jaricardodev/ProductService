# Product Service

This service holds logic related to Product specific domain


## Getting Started
Clone source code from git repo.
Make sure you have .Net 5 installed.
 Make sure to have access to Nuget.org for nuget packages restore

## Build & Run

Once project is cloned open ProductService.sln. and rebuild. Nuget pacakges should be restored and no compilation errors or warnings should appear.
Once soulution is built, Run it with visual studio or using the ProductService.exe on output folder (typically bin\Debug\net5.0\ProductService.exe). 
If you use visual studio make sure to select the option  "ServiceHost" on the Run button instead the "IIS Express" one. Both should work, but this documentation is based on running the service as self-hosted and not as IIS application.

## Test it

To test the API endpoints you can access the swagger on the by navigating on the browser to http://localhost:9010/index.html 
The endpoints are protected by JWT bearer toke authorization. you can use the following token for testing purposes: 
>eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJzb21lSXNzdWVyIiwiaWF0IjoxNjI0MzE0ODIzLCJleHAiOjE3MjQzMTUzMjMsImF1ZCI6ImFsbCIsImFkbSI6dHJ1ZX0.K1Sue_9egvw-yzQ9yskRADQl2BXEp0g3n_LPN9A2VMs

You can easily customize token content on https://jwt.io/. 
The secret key can be found on the appsettings.json file within the ServiceHost project folder. The app settings.json file should be created automatically for you on initial build, but in case is not, you can create one using the content on  sample_appsettings.json file.

## Metrics
To acces the API metrics navigate on browser to http://localhost:9010/metrics
It uses the most basic implementation of a popular library called [App Metrics](https://www.app-metrics.io/getting-started/) 

## Solution structure

The solution contains 5 projects

- Model: Contains all domain logic and core application capabilities
- Model.Tests: Contains tests for Model project
- Persistence: Contais data access logic
- Persistence.Tests: Contains tests for Persistence project
- ServiceHost: Contains service configuration and startup logic and API endpoints

## Architecture highlights

The Product service attempt to follow a simplified [DDD](https://martinfowler.com/bliki/DomainDrivenDesign.html) approach. In order to understand its structure and proper usage keep in mind the following key points:

### Operation entities are located on Model/Operations and the represent business concepts:
1. They all must inherit OpertaionEntity class.
2.  Primary responsibility of an entity is to maintain its continuity. Define the identity on an entity so that it can be effectively tracked.
5.  Keep the class definition of an entity simple. Add only requisite intrinsic attributes and behaviors. Extract anything unessential into objects associated with the focal entity.
6.  Entity objects are compared by identity regardless of their form or history. Avoid matching entities by attributes.

### Value objects are defined inside Model/Capabilities folder and they represent a descriptive aspect of the domain with no conceptual identity:
1.   Never define an identity for a value object. Its class definition and responsibilities should never depend on a thread of identity and continuity, which usually results in additional analytical work on the object life management. The behavior of a value object depends on what this object is, not who or which it is.
2.  Value objects are matched by a state operator. Consider overriding equality methods to achieve proper matching using object attributes, or use records.   
3.  Keep value objects immutable -- changeable only by full replacement. This makes the program run both safer and faster.     

### Services are defined inside Model/Services and represent a domain operation that does not conceptually belong to any object:
 1.  Service relates to a domain concept that is not natural to model as a domain object. As complex operations draw together many domain objects, coordinating them and putting them into action, they would easily obscure an object’s role.   
2.  Define an interface for a service in terms of other elements of the domain model. Name a service after an activity as opposed to an entity. 
3.  Make a service stateless. Its interface stands alone in the model.  
4.  Differentiate between Application and Domain services. Domain operations embed significant business rules and use meaningful business terms. 

### Repositories abstractions are defined inside Model/Repositories an represent an access mechanism to the domain entities:
 1. Repositories implementations will be defined on Persistance/Repositories folder.   
2.  Avoid creating a repository per domain entity or relational entity, they rather should be associated to a domain activity and the data access required by it.
3.  Domain entities will determine input and output data for Repositories, but they not necessarily need to match the relational entities. Use mapper definitions to do conversions, they can be defined on Persistance/Mappers folder

# How it works?:

The API endpoints logic will be very simple for instance to create a product the PrductController class only do:
```c#
		public async Task<IActionResult> Post(Product product)
        {
            var createdProduct = await ProductCatalogService.CreateAsync(product);
            return CreatedAtAction("Get", new { id = createdProduct.Id }, createdProduct);
        }
```  
The main responsibility for the product creation relies on the IProductCatalogService injected to the controller via dependency injection on the ServiceHost/StartUp class:
```c#
services.ConfigureModelServices();
services.ConfigurePersistenceServices();
```
Both method are IServiceCollection extensions and can be found on ServiceHost/Extensions/IServiceCollectionExtension class: 
```c#
	public static class IServiceCollectionExtension
    {
        public static void ConfigureModelServices(this IServiceCollection services)
        {
            services.AddScoped<IProductCatalogService, ProductCatalogCatalogService>();
            services.AddSingleton(new ValidatorFactory());
        }

        public static void ConfigurePersistenceServices(this IServiceCollection services)
        {
            services.AddScoped<IProductCatalogRepository, DBProductCatalogRepository>();
        }
    }
```
Mostly the dependency injection related to our domain will be added on this class.
The IProductCatalogService implementation has the following main responsabilities:

- Apply business rules/validations to a Product
```c#

        public async Task<Product> CreateAsync(Product product)
        {
            var validationResult = ValidatorFactory.GetValidator(product).Validate();
            if (!validationResult.IsValid)
                throw new InvalidProductException(validationResult.Message);
			....
        }
```
- Order repository to persist the Product, including certain tolerance for transient failure on the data access
```c#
		public async Task<Product> CreateAsync(Product product)
        {
            ...
	            product.Id = await Policy.Handle<Exception>()
                .WaitAndRetryAsync(2,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(5, retryAttempt)),
                    (exception, timeSpan, retryCount, context) =>
                    {
                        Logger.LogError(exception, "Adding Product attempt {Attempt} error.", new { Attempt = retryCount });
                    })
                .ExecuteAsync(async () => await ProductCatalogRepository.AddAsync(product));

            ...
        }
```
Validation mechanisms works using a simple Specification pattern. For product we have a simple specification:
```c#
	public record ProductNameMustBeSpecified(Product Product) : ISpecification
    {
        public bool IsSatisfiedBy() => !string.IsNullOrWhiteSpace(Product.Name);

        public string ErrorMessage() => "The product name is required";
    }
```
which is part of the ProductValidator:
 ```c#
	public record ProductValidator : Validator
    {
        public ProductValidator(Product product)
        {
            Add(new ProductNameMustBeSpecified(product));
        }
    }
```
And this last one is provided to the IProductCatalogService via an injected factory:
```c#
	public record ValidatorFactory
    {
        public Validator GetValidator(OperationEntity entity)
        {
            return entity.GetType() switch
            {
                var productType when productType == typeof(Product) => new ProductValidator((Product) entity),
                _ => throw new ArgumentOutOfRangeException(entity.GetType().Name)
            };
        }
    }
``` 
Transient fault tolerance on persisting the Product is handled by a library called [Polly](https://github.com/App-vNext/Polly) 
Repositories implementation inherit both  the domain definition and a Base repository created to share common logic for data access:
 ```c#
	 public class DBProductCatalogRepository : BaseRepository, IProductCatalogRepository
    {
	    ...
	    public async Task<int> AddAsync(Model.Operations.Product product)
        {
            var dbProduct = product.Adapt<Product>();
            await base.AddAsync(dbProduct);
            return dbProduct.Id;
        }
        ...
    }
 ```
 ```c#
  public abstract class BaseRepository
    {
        ...
        public virtual async Task AddAsync<TEntity>(TEntity entity) where TEntity : class
        {
            Context.Set<TEntity>().Add(entity);
            await Context.SaveChangesAsync();
            Context.Entry(entity).State = EntityState.Detached;
        }
			...
    }
 ```