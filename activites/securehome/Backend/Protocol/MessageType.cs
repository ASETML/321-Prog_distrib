namespace Backend.Protocol;

public enum MessageType
{
    HELLO,                      // Startup announcement
    GOOD_BYE,                   // Shutdown announcement
    TIME_SYNC,                  // Time synchronization
    THREAT_ENVIRONMENT,         // Threat info from ThreatSimulator
    HOUSE_SECURITY_STATUS,      // House status data
    SECURITY_STATUS_REQUEST,    // Request status from house

    // TODO 02: Définir les types de messages pour les alertes de sécurité
    THREAT,
    // TODO 06: Définir les types de messages pour les vérifications d'alertes
    THREAT_CHECK,
    // TODO 11: Définir les types de messages pour les commandes à distance
    REMOTE_COMMAND,
    THREAT_RECEIVED // Confirmation de réception
}
