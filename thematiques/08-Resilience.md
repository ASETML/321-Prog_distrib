# Résilience ou Design for failure
Comment faire en sorte que les pannes soient une donnée contextuelle plutôt qu’une source
de désastre...

# Théorie
- [Design for failure](../supports/design4failure.md)
- [Kahoot p2p/d4f](https://create.kahoot.it/share/p2p-et-resilience-des-reseaux/20a4a666-6729-4382-8a91-aa8eb8ec1984)

# Pratique
- [Powercher](https://github.com/ETML-INF/powercher) : 
  - Ajouter des timeouts pour les messages de deals
  - Implémenter [Exponential Backoff](../supports/design4failure.md#exponential-backoff-pour-retryjitter)
