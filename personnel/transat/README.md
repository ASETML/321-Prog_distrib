# Transactions... distribuées

## Transactions
Pour rappel, une transaction a pour but de garantir une *livraison*, un peu comme un recommandé postal...

## Environnement distribué
Une transaction distribuée implique plusieurs livraisons dans une transaction.

### Exemple
Dans [Powercher](https://github.com/ETML-INF/powercher) on utilise un bus MQTT qui ne supporte pas nativement un 
protocole transactionnel comme [2PC](https://martinfowler.com/articles/patterns-of-distributed-systems/two-phase-commit.html)...

De plus, dans une implémentation future on peut facilement imaginer qu'on utilisera une base de données qui ne supporte
pas forcément non plus 2PC ou alors simplement pour la gestion de l'idempotence, on pourrait ne pas trop faire confiance
au système de fichier dans lequel on stockerait les `ids` déjà traités...

Bref, il est pertinent de s'entraîner à gérer un système faillible `à la main` et c'est la mission qui vous est confiée.

## Contexte
Vous devez donc dupliquer la classe `NonResilientPublisher` du projet [transat](https://github.com/jonathanMelly/transat) et la nommer `ResilientPublisher`
sans toucher au `storage` ou au `programme principal` afin que celui rapporte un 100% de réussite.

## Livraison
Un pull request (qui ne sera pas pris en compte pour garder l'exercice sans la solution) avec la correction et dans
lequel il y a un screenshot d'une éxécution réussie.
