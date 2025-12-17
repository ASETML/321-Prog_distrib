# SecureHome - Smart Home Security Monitoring System

## Architecture

```
┌─────────────────┐         ┌──────────────────┐         ┌─────────────────┐
│  House Security │◄───────►│ In-Memory PubSub │◄───────►│ Monitoring      │
│     Console     │         │   Message Bus    │         │    Center       │
└─────────────────┘         └──────────────────┘         └─────────────────┘
        ▲                            ▲
        │                            │
        │                   ┌────────────────┐
        └───────────────────│     Threat     │
                            │   Simulator    │
                            └────────────────┘
```

## Components

1. **House Security Console** - Manages home security sensors and responds to monitoring center
2. **Monitoring Center** - Receives alerts and can send verification requests/commands
3. **Threat Simulator** - Generates random intrusion events (mimics real-world threats)
4. **In-Memory Message Bus** - Simple publish/subscribe communication (no networking)