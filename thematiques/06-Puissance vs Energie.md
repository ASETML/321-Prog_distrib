# Puissance et énergie

## Théorie

- [Explications](../supports/Puissance%20et%20énergie.md)
- [Kahoot recap](https://create.kahoot.it/details/869bfcbc-6e29-414a-82eb-8764bef04efd)

## Pratique

Maintenant que vous savez que l'énergie se calcule en multipliant la puissance en `[kW]` par le temps en `[h]`, vous allez pouvoir faire le bilan énergétique de votre maison, en appliquant l'algorithme suivant:

```

newTime = nouvelle date/heure reçue de Mère Nature
timeDiff = différence entre newTime et la date/heure reçue avant
durée = conversion de timeDiff en nombre décimal
TotalConsommation = 0
Pour chaque consommateur en fonction
    TotalConsommation += durée * puissance de l'appareil
TotalProduction = 0
Pour chaque producteur en fonction
    puissanceActuelle = puissance de l'appareil ajustée en fonction de la météo
    TotalProduction += durée * puissanceActuelle
BilanEnergétique = TotalProduction - TotalConsommation

```

Faites en sorte que votre maison affiche son bilan énergétique, recalculé à chaque message de Mère Nature
