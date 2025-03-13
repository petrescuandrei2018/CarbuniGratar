namespace CarbuniGratar.Web.Models
{
    public enum StatusCosDeCumparaturi
    {
        CosFaraProduse, // Clientul încă nu a adăugat nimic
        CosCuProduse, // Există produse în coș, dar nu a fost trimis
        CosFinalizatDeClientTrimisLaVanzator, // Clientul a finalizat coșul și l-a trimis la vânzător
        CosAprobatSiInLivrare // Vânzătorul a confirmat și trimis coșul către livrare
    }
}
