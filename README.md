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
                <li><a href="#graph-property-name">GraphPropertyName</a></li>
                <li><a href="#graph-property-name">GraphPropertyNameBehavior</a></li>
            </ul>
        </li>
      </ul>
    </li>
    <li><a href="#roadmap">Roadmap</a></li>
    <li><a href="#contributing">Contributing</a></li>
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
Any Linq method not supported and defined above will be skipped by the engine

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


<!-- ROADMAP -->
## Roadmap

Actually there is no roadmap really defined, i will give some updates occasionally to add other broker support and to add some unit tests.

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

<!-- CONTACT -->
## Contact

Benjamin Mandervelde - [@kakktuss](https://twitter.com/Kakktuss) - benjaminmanderveldepro@gmail.com
