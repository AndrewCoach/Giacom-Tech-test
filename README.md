The implementation for all four tasks can be found on the feature/order-management-endpoints branch. I have structured the work into a series of logical commits with maybe a bit more description than normal.

I tried to follow a clean, layered architecture, ensuring separation of concerns between the data access, service logic, and API controller layers. Each new feature was accompanied by corresponding unit tests. 
For ease of testing, I also integrated Swagger, which provides an interactive API documentation page in a development environment.

The most significant technical challenge encountered was a limitation within the MySql.EntityFrameworkCore database provider. 

Initially, a complex LINQ query for calculating monthly profit could not be translated into SQL by the provider, resulting in a runtime error. To overcome this, I refactored the query. New approach involves fetching the necessary raw data with a simpler, translatable query and then performing the complex grouping and aggregation in-memory. While this involves a performance trade-off, it was the most robust and pragmatic solution to work around the provider's limitations. Not ideal for production with large databases though.

Issue with forwarding for production version was fixed after excrutiating pain by adding missing mapping to program.cs.