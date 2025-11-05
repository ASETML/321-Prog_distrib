# Sérialise ma série

Voici le pitch de la saison 1 de la série "Eté et miel":

> Gérard et Maya dirigent la cafétéria d'une école professionnelle. Leur spécialité: les endives aux miel qui s'arrachent chaque jour à leur comptoir. Leur fournisseur de miel a décidé de tripler ses prix, Gérard et Maya sont pris au piège.
>
> Un groupe de profs et d'élèves de l'école décident de leur venir en aide en se lançant dans l'apiculture pour produire leur précieux nectar en cachette dans la salle des serveurs de l'école... 

## Etape 1

Créez une application console (!! **SANS** .NET Framework !!)

Chacun crée un objet de la classe Character pour un personnage différent. Choisissez Gérard ou Maya, ou inventez un personnage fictif (en rapport avec le pitch de la série).

```csharp
public class Character
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Description { get; set; }
    public Actor PlayedBy { get; set; }
}
```
Le champ Actor reste `null`.

Sérialisez votre objet en JSON, déposez le résultat (par exemple `gérard.json`)dans un dossier partagé (Teams)

Récupérez les fichiers des autres, dé-sérialisez-les tous !

## Etape 2

Faites pareil avec la classe `Actor`:

```csharp
public class Actor
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime BirthDate { get; set; }
    public string Country { get; set; }
    public bool IsAlive { get; set; }
}
```

## Etape 3

Faites jouer votre personnage par votre acteur: 

```csharp
// TODO Désérialiser un seul fichier

Console.WriteLine($"Le personnage de {perso.FirstName} {perso.LastName} est joué par {perso.PlayedBy.FirstName} {perso.PlayedBy.LastName}");
```

Résultat attendu, quelque chose comme ça:
```
Le personnage de Melissa D'Anjou est joué par Jennifer Aniston
```

## Etape 4

Inventez un épisode de la saison:

```csharp
public class Episode
{
    public string Title { get; set; }
    public int DurationMinutes { get; set; }
    public int SequenceNumber { get; set; }
    public string Director { get; set; }
    public string Synopsis { get; set; }
    public List<Character> Characters { get; set; } = new List<Character>();
}
```
(et partagez-le!)

## Etape 5

Assemblez la saison 1 de "Eté et miel" dans un seul fichier

## Suite pour celles et ceux qui sont en avance
Reprendre l’exercice avec d’autres formats :
- [XML](https://learn.microsoft.com/en-us/dotnet/standard/serialization/examples-of-xml-serialization)
- [ProtoBuf](https://learn.microsoft.com/en-us/aspnet/core/grpc/protobuf?view=aspnetcore-8.0)

## Conclusion

Mise en commun: qu'avez-vous utilisé pour sérialiser/désérialiser ?  
Décidons ensemble de l'outil que nous utiliserons tous à l'avenir.