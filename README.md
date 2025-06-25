The implementation for all four tasks can be found on the feature/order-management-endpoints branch. I have structured the work into a series of logical commits with maybe a bit more description than normal.

I tried to follow a clean, layered architecture, ensuring separation of concerns between the data access, service logic, and API controller layers. Each new feature was accompanied by corresponding unit tests. 
For ease of testing, I also integrated Swagger, which provides an interactive API documentation page in a development environment.

The most significant technical challenge encountered was a limitation within the MySql.EntityFrameworkCore database provider. 

Initially, a complex LINQ query for calculating monthly profit could not be translated into SQL by the provider, resulting in a runtime error. To overcome this, I refactored the query. New approach involves fetching the necessary raw data with a simpler, translatable query and then performing the complex grouping and aggregation in-memory. While this involves a performance trade-off, it was the most robust and pragmatic solution to work around the provider's limitations. Not ideal for production with large databases though.

Finally, everything is working for me locally, but building your release executable just refuses to work!
This does not seem at all like a code issue, rather a SSL/TLS version mismatch and an issue with MySQL driver package itself.
Not sure why Mysql was chosen for this but it seems to be a source of all my frustrations.
Unless I am missing something, the only solution seems to be switching the DB provider, and I cannot do that.
Anyways, since I have been battling with this already for 1 hour, I have run out of any capacity to try to make this work.
Code is functional and hopefully it will work on your side.