# sqrach

Sqrach is a syntax highlighted SQL editor with intelligent autocomplete functionality to help you minimize typing and flush out ideas quickly.  Here is a brief description of the projects in the solution.

* sqrach/sqrach

   SQL Editor Windows Forms application.

* lib/dbInfo

   Routines that gather structure of database including foreign keys if possible.  Does basic data analysis to figure out things like distribution of data and cardinality.

* lib/sqlparser

   Routine that parse SQL.

* lib/mysql, lib/mssql, lib/sqlite

   Thin libraries for connecting and working with data in databases.

* lib, lib.forms

   Libraries, commonly used routines

