
# Date : 26-08-2024
# Test App for the WebAPI (REST)
# TargetFramework Net6.0
# Include Packages 
#  - "Dapper" Version="2.1.35"
	Dapper is a lightweight, high-performance data access tool built by the Stack Overflow team. 
	It is a lightweight, high-performance micro-ORM (Object-Relational Mapper)
	Dapper supports a wide range of database providers, including SQL Server, MySQL, PostgreSQL, and more
	Dapper allows you to execute queries that return multiple result sets using the QueryMultiple method.
	This is useful when you need to retrieve data from multiple tables or stored procedures in a single database call.

#  - "Polly" Version="8.4.1", Polly.Core" Version="8.4.1
	We are using this for Retry policy lets you retry a failed request due to an exception or an unexpected or 
	bad result returned from the called code. It doesn’t wait before retrying

