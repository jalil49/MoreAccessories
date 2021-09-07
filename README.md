# Overhaul of MoreAccessories
Original author of MoreAccessories: Joan6694
## What is the purpose of the overhaul?

The purpose of the overhaul is to make plugin modding that uses accessories easier.

Previously MoreAccessory data is stored separately from the main 20 accessories.

The overhaul expands the fixed 20 arrays in multple places and as such requires more fine tuning.

## Noteable changes
In maker the slots showed will be based on the largest coordinate while in maker to avoid having to do weird stuff just to copy and transfer accessories.

Scroll bars have been added for accessory slots scroll and H accessory list

MoreAccessory directly adds the scrolling to the accessory window (previously implemented in KKAPI)

Excess slots are trimmed when saving, unfortunately I can't compress them since it would be breaking plugins.
