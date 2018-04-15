using Mono.Addins;
using Mono.Addins.Description;
using Mutatio;

[assembly: Addin(
    "Mutatio",
    Namespace = "Mutatio",
    Version = "1.0.2"
)]

[assembly: AddinName("Mutatio")]
[assembly: AddinCategory("IDE extensions")]
[assembly: AddinDescription("Automatically convert projects from PCL to .NET Standard 2.0.")]
[assembly: AddinAuthor("Evgeny Zborovsky")]
[assembly: AddinUrl(Consts.ProjectUrl)]