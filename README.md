<!-- PROJECT LOGO -->
<h3 align="center">LinqToGraphQL</h3>
<p align="center">
    <br />
    <br />
    <a href="https://github.com/Kakktuss/HotMessageBus/issues">Report Bug</a>
    ·
    <a href="https://github.com/Kakktuss/HotMessageBus/issues">Request Feature</a>
</p>


<!-- TABLE OF CONTENTS -->
<details open="open">
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#installing">Installing</a></li>
      </ul>
    </li>
    <li>
      <a href="#usage">Usage</a>
      <ul>
        <li>
            <a href="#graphset">GraphSet</a>
            <ul>
                <li><a href="#operators">Operators</a></li>
                <li><a href="#aggregators">Aggregators</a></li>
            </ul>
        </li>
        <li>
            <a href="#graphcontext">GraphContext</a>
            <ul>
                <li><a href="#graphset-construction">GraphSet Construction</a></li>
                <li><a href="#graphset-configuration">GraphSet Configuration</a></li>
            </ul>
        </li>
        <li>
            <a href="#attributes">Attributes</a>
            <ul>
                <li><a href="#global-attributes">Global attributes</a></li>
                <li><a href="#method-attributes">Method attributes</a></li>
                <li><a href="#method-parameter-attributes">Method parameter attributes</a></li>
                <li><a href="#property-attributes">Property attributes</a></li>
            </ul>
        </li>
      </ul>
    </li>
    <li>
      <a href="#example">Example</a>
    </li>
    <li><a href="#roadmap">Roadmap</a></li>
    <li><a href="#contributing">Contributing</a></li>
    <li><a href="#thanks">Thanks</a></li>
    <li><a href="#contact">Contact</a></li>
  </ol>
</details>

<!--ABOUT THE PROJECT-->
## About the project

This project has been built on the idea to make an easy to use and simple graphql client for C# users who enjoy interacting with IQueryables.
<br/>
To build this client i've been widely inspired by EF Core syntax using DbContext and DbSet. That's why you will surely notice the similarity between the concepts I introduced int the lib and ef core.

<!--GETTING STARTED-->
## Getting Started

### Installing

To install this library just reference it using nuget package manager:

````
dotnet add package LinqToGraphQL
````

<!-- USAGE -->
## Usage

To start using this library, let me introduce some concepts:

- GraphSet: This is the main entry point on a data set. It derives from the class IQueryable and contains all the logic to query a GraphQL Api
  - GraphSetConfiguration: This class is the configuration holder for a specific GraphSet
    - GraphSetHttpConfiguration: This class is the http configuration holder for a specific GraphSet
    - GraphSetQueryConfiguration: This class is the query configuration holder for a specific GraphSet
- GraphContext: This is where you define every request to send to a GraphQL Api

These classes are the only classes available to use to build your graphql querying logic.

<!--GraphSet-->
## GraphSet

Before defining a GraphContext we must understand what's the GraphSet, how does it works & what operators (and aggregators) does it support ?

As already said before, the GraphSet class is the main entry point on a data set, as it derives from the class IQueryable.
Actually, this class only supports 3 server-side operators and 4 aggregators supported (more coming soon):

### Operators

- Include: This operator is defined to include a sub IEnumerable (which represents a many-to-many relationship) on the Entity.
  - This operator can be used on either a simple IEnumerable Property (ie: Posts) or a Method that returns an IEnumerable (ie: Posts())
  - If used on a method that returns an IEnumerable, the method arguments will be translated to GraphQL inputs (ie: ``Posts(int a, int b)`` will be translated to `posts(a:$postsA,b:$postsB) {  }`)
- ThenInclude: This operator is also defined to include a sub IEnumerable or a simple property on the Entity previously included used Include
  - This operator has the same logic of Include one which means that it works on IEnumerable properties, methods that returns an IEnumerable and any other "simple" property (ie: int, string, etc...)
  - If this operator is used on a simple property, this property will be added as an included property (ie: `Include(e => e.Posts(int a, int b).ThenInclude(e => e.Name)` will be translated to `posts(a:$postsA,b:$postsB) { Name }`)
- Select: This operator is defined to include only simple properties on an Entity. It can be used both on the base entity of the GraphSet or included entities with Include or ThenInclude (only if these entities are of type IEnumerable)

For example, here's a simple code:
````cs
IQueryable<User.User> query = testContext.User()
	.Select(e => new User.User
	{
		Name = e.Name,
		Username = e.Username
	})
	.Include(e => e.Posts(1, 2))
		.Select(e => new Post.Post()
		{
			Content = e.Content,
			Title = e.Title
		})
		.ThenInclude(e => e.Comments(1, 2))
			.Select(e => new Comment.Comment
			{
				Content = e.Content,
				Title = e.Title
			});
````

This query will be translated to 
````cs
query ($postsA:Int, $postsB:Int, $postsCommentsA:Int, $postsCommentsB:Int){
  Name,
  Username,
  Posts(a: $postsA, b: $postsB) {
    Content,
    Title,
    Comments(a: $postsCommentsA, b: $postsCommentsB) {
      Content,
      Title
    }
  }
}
````

### Aggregators

Non-asynchronous aggregators: 
- ToList: Used to return the result of the query as a IList<T>
- ToItem: Used to return the result of the query as a simple item T

Asynchronous aggregators:
- ToListAsync: Used to return the result of the query as a IList<T>
- ToItemAsync: Used to return the result of the query as a simple item T

### How to use GraphSet

When using the GraphSet you must keep in mind that while you don't call any built-in or Linq aggregators, the query will never be executed/parsed by the Translator and the QueryProvider.
<br/>
To trigger the request you must use one of these aggregators defined above or those available through Linq extension methods.
<br/>
As soon as you call these aggregators the result will be stored in memory and other Linq extension methods will be available.
Any Linq non-aggregator methods not supported and defined above will be skipped by the server-side engine and executed in-memory on the resulting IEnumerable.

````cs
IQueryable<User.User> query = testContext.User()
	.Select(e => new User.User
	{
		Name = e.Name,
		Username = e.Username
	})
	.Include(e => e.Posts(1, 2))
		.Select(e => new Post.Post()
		{
			Content = e.Content,
			Title = e.Title
		})
		.ThenInclude(e => e.Comments(1, 2))
			.Select(e => new Comment.Comment
			{
				Content = e.Content,
				Title = e.Title
			});

var resultList = query.ToList();
````

## GraphContext

Now that we know how a GraphSet works we can now turn to the GraphContext.
As said before, the GraphContext is a class that you must inherit in order to use the GraphSet.
<br/>
This context is used to define every request you can use on a GraphQL API.
This is also were you can register GraphSet configuration classes inside a specific method named ``Configure`` (which is a protected virtual void to be overrided if needed by the Context)

### GraphSet Construction
To construct a GraphSet, there's a method Set<T> in the GraphContext which returns a GraphSet<T>.
This method takes 2 arguments:
- parameterValues: The base input parameters of the query/mutation to apply
- graphSetConfigurationAction: A configuration builder used to extend the base configuration of the requested Set or to create it without calling ConfigureSet in the Configure method

Here's an example of the Set method:
`````cs
[GraphPropertyName("countries")]
public GraphSet<Country.Query.Country> Countries(string authorization)
{
    return Set<Country.Query.Country>(Array.Empty<object>(), builder =>
    {
        builder.ConfigureHttp(httpBuilder =>
        {
            httpBuilder.ConfigureHeaders(headers =>
            {
                headers.Authorization = new AuthenticationHeaderValue("Bearer", authorization);
            });
        });
    });
}
`````

### GraphSet Configuration
In order to configure the behavior of a GraphSet, you have two possibilities:
- The first one is to use the graphSetConfigurationAction variable to build an entire configuration/replace some base configurations to a particular request (ie: if you have two endpoints which results in the same entity (so the same GraphSet) but one requires an authorization token, you can register it using this variable)
- The second one is to call override the Configure method in your GraphContext definition and make a call to ConfigureSet<T>() which also takes a GraphConfigurationBuilder as a variable to build the set configuration

Here's a full combination of both possibilities: 
````cs
[GraphPropertyName("countries")]
public GraphSet<Country.Query.Country> Countries(string authorization)
{
    return Set<Country.Query.Country>(Array.Empty<object>(), builder =>
    {
        // Can also be called here - will replace the base value defined in Configure
        builder.WithUrl("secondUrl");
    
        builder.ConfigureHttp(httpBuilder =>
        {
            httpBuilder.ConfigureHeaders(headers =>
            {
                headers.Authorization = new AuthenticationHeaderValue("Bearer", authorization);
            });
        });
    });
}

protected override void Configure(GraphContextConfigureOptionsBuilder graphContextConfigureOptionsBuilder)
{
    graphContextConfigureOptionsBuilder.ConfigureSet<Country.Query.Country>(builder =>
    {
        builder.WithUrl("url");

        builder.ConfigureHttp(httpBuilder =>
        {
            httpBuilder.WithMethod(HttpMethod.Post);
        });
    });

}
````

<!-- Attributes -->
## Attributes
In order to have a full control of how the query is translated by the engine, there's actually two Graph attributes which you can use to modify the behavior of the engine.
These attributes have the ability to be used on Entity definitions, Context method definitions and input definitions.

### Global attributes

#### GraphPropertyName
This attribute can be used to configure the name of the member translated into the query. It can be used either on methods; method parameters or properties
<br/>
It takes a name constructor argument which is a string.

Example:
````cs
[GraphPropertyName("countries")]
public void Countries() {
}
````

Will be translated to:
`query { countries {  } }` instead of `query { Countries {  } }`

Another example this time with method parameters:
`````cs
public void Countries([GraphPropertyName("inputA")] string a) {}
`````

Will be translated to:
`query($a:String) { Countries(inputA: $a) {  } }` instead of `query($a:String) { Countries(a: $a) {  } }`

#### GraphPropertyNameBehavior
This attribute can be used to configure the way the name will be displayed into the query or the mutation. It can be used either on methods; method parameters or properties
<br/>
It takes a propertyBehavior which is an Enum of TranslatorBehavior:
- CamelCase: This behavior will display the name in camelCase
- UpperCase: This behavior will display the name in UPPERCASE
- LowerCase: This behavior will display the name in lowercase

````cs
[GraphPropertyNameBehavior(TranslatorBehavior.UpperCase)]
public void Countries() {
}
````

Will be translated to:
`query { COUNTRIES {  } }` instead of `query { Countries { } }`

### Method attributes

#### GraphBackingField
This attribute can be used to specify a backingfield container which will contain the resulting JSON data of the request executed.
It takes a propertyName which is a string and should be a nameof() of a property inside the class.

````cs
private List<CountryLocale> _countryLocales;

[GraphBackingField(nameof(_countryLocales))]
public List<CountryLocales> CountryLocales() => _countryLocales;
````

### Method parameter attributes

#### GraphNonNullableProperty
This attribute can be used only on method parameters as it acts as a Non-nullable type indicator.

````
public void Countries([GraphNonNullableProperty] int countryId) {
}
````

#### GraphParameterType
This attribute can be used only on method parameters as it acts as a type-changing indicator.
This attribute changes the name displayed on the query inputs declaration. 
<br/>
It can be useful in some cases where a type can be a custom scalar input defined by the API but his value need to be a c# built-in type such as "string".
Example: <https://shopify.dev/api/admin/graphql/reference/scalar#id-2021-04>; Here ID is a custom scalar but his value is a string in the query.

````cs
public void Posts([GraphPropertyType(typeof(string))] Guid id) {}
````

The current input declaration will be translated to:
`query ($postsId:String) { posts(id:$postsId) {  } }`

### Property attributes

#### GraphUnionTypeProperty
This attribute can be used only on property as it acts as a Union-type indicator.
As you may know, in the GraphQL spec there's a part where you can define a union type which represents multiple types.
This attribute must be used to define what types an object can be.

/!\ Please keep in mind that with this attribute, the Property must be an Object

````cs
[GraphUnionTypeProperty(typeof(ObjA), typeof(ObjB), typeof(ObjC) /*, etc....*/)
public object UnionProperty { get; set; }
````

<!-- Example -->
## Example
Here's a full example (accessible in the example folder) of an Entity definition and his translation by the query engine:

Here's the entities definitions:
````cs
public class User
{
    [GraphPropertyName("name")]
    public string Name { get; set; }
    
    [GraphPropertyName("username")]
    public string Username { get; set; }

    private List<Post.Post> _posts;

    [GraphPropertyName("posts")]
    [GraphBackingField(nameof(_posts))]
    public List<Post.Post> Posts([GraphNonNullableProperty] [GraphPropertyName("postsId")] int id) => _posts;
    
    [GraphPropertyName("comments")]
    public List<Comment.Comment> Comments { get; set; }
}

public class Post
{
    [GraphPropertyName("title")]
    public string Title { get; set; }
    
    [GraphPropertyName("content")]
    public string Content { get; set; }

    private List<Comment.Comment> _comments;

    [GraphPropertyName("comments")]
    [GraphBackingField(nameof(_comments))]
    public List<Comment.Comment> Comments([GraphPropertyName("commentsId")] int id) => _comments;
}

public class Comment
{
    [GraphPropertyName("title")]
    public string Title { get; set; }
    
    [GraphPropertyName("content")]
    public string Content { get; set; }
}
````

And the context definition:
````cs
public class UserContext : GraphContext
{
    [GraphPropertyName("user")]
    [GraphPropertyNameBehavior(TranslatorBehavior.UpperCase)]
    public GraphSet<User.User> User([GraphNonNullableProperty] string username)
    {
        return Set<User.User>(new object[]
        {
            username
        }, builder =>
        {
            builder.ConfigureQuery(queryBuilder =>
            {
                queryBuilder.WithType(GraphSetTypes.Query);
            });
        }); 
    }

    protected override void Configure(GraphContextConfigureOptionsBuilder graphContextConfigureOptionsBuilder)
    {
        graphContextConfigureOptionsBuilder.ConfigureSet<User.User>(builder =>
        {
            builder.WithUrl("https://example.com/graphql");

            builder.ConfigureHttp(httpBuilder =>
            {
                httpBuilder.WithMethod(HttpMethod.Post);
            });
        });
    }
}
````

Now that we defined the entities & the context we can create our query:
````cs
var userContext = new UserContext();

IQueryable<User.User> userQuery = userContext.User("username")
    .Select(e => new User.User
    {
        Name = e.Name,
        Username = e.Username
    })
    .Include(e => e.Posts(10))
        .Select(e => new Post.Post()
        {
            Content = e.Content,
            Title = e.Title
        })
        .ThenInclude(e => e.Comments(10))
            .Select(e => new Comment.Comment
            {
                Content = e.Content,
                Title = e.Title
            });

var user = userQuery.ToItem();
````

This query once executed using the aggregator ToItem will be translated to: 
`{"query":"query ($username:String!, $postsId:Int!, $postsCommentsId:Int){ result: USER(username:$username) { name, username, posts(postsId:$postsId) { content, title, comments(commentsId:$postsCommentsId) { content, title } } } }","variables":{"username":"username","postsId":10,"postsCommentsId":10}}`

<!-- ROADMAP -->
## Roadmap

Actually there is no roadmap really defined, i will give some updates occasionally to add other linq operators support and to add some unit tests.

<!-- CONTRIBUTING -->
## Contributing

Contributions are what make the open source community such an amazing place to be learn, inspire, and create. Any contributions you make are **greatly appreciated**.

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes depending on [Conventional Commit]("https://www.conventionalcommits.org/en/v1.0.0/") spec (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

<!-- LICENSE -->
## License

Distributed under the MIT License. See `LICENSE` for more information.

<!-- THANKS -->
## Thanks

A big thanks to [@Giorgi](https://github.com/Giorgi)'s lib [GraphQLinq](https://github.com/Giorgi/GraphQLinq) which helped me to start this project.
<br/>
And another special thanks to [@Mattwar](https://github.com/mattwar) with his github repository [IQToolkit](https://github.com/mattwar/iqtoolkit/tree/master/docs/blog) which helped me to understand how Linq & IQueryable work in depth.  

<!-- CONTACT -->
## Contact

Benjamin Mandervelde - [@kakktuss](https://twitter.com/Kakktuss) - benjaminmanderveldepro@gmail.com
