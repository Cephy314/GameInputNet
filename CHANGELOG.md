## Unreleased

## v1.0.3 (2025-11-12)

### Fix

- updated action to use .net 8

## v1.0.2 (2025-11-12)

### Feat

- bumped to .NET 8.0 - Windows

## v1.0.1 (2025-11-12)

### Fix

- added LICENSE file and updated readme

## v1.0.0 (2025-11-12)

### Feat

- Added auto deploy and version
- Adds example project for GameInput.Net
- Adds GameInput creation method and improves callbacks
- implemented managed dispatcher.
- impl. smoke test for FindDeviceByPlatformString
- added helper to convert native GameInput to Managed
- Implemented FindDeviceById() to managed layer
- Implement Create/Disable Aggregate Device methods
- Add infrastructure to parse GameInput.h COM interface definitions

### Fix

- Improved Example
- Removed empty attributes
- **examples**: Refactored to display enumerated devices
- reverting accidental conversion on Virtualkey
- smoke tests skip when no hardware available

### Refactor

- Improved AppLocalDeviceId functionality
- Clean up COM object handling and enumeration in GameInput
