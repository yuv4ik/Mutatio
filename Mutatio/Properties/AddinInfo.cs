using Mono.Addins;
using Mono.Addins.Description;

[assembly: Addin(
    "Mutatio",
    Namespace = "Mutatio",
    Version = "1.0"
)]

[assembly: AddinName("Mutatio")]
[assembly: AddinCategory("IDE extensions")]
[assembly: AddinDescription("Converts PCL to .NET Standard 2.0 automatically.")]
[assembly: AddinAuthor("Evgeny Zborovsky")]
[assembly: AddinUrl("https://github.com/yuv4ik/mutatio")]