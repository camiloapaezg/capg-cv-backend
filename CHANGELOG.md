# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.0.3] - 2026.10.07

### Added

- Retry and circuit breaker strategies applied to processes involving files scanning and deleting.

### Fixed

- When an user is deleted, the corresponding files and metadata should be also deleted.
- File metadata must be deleted only after removing the actual file in blob.

## [0.0.2] - 2026.10.01

### Security

- Updated dependencies to address security vulnerabilities.

### Changed

- Unit and integration tests updated according to new dependencies.
- Dockerfile updated to be deployed in local Docker compose environment.
- Files Upload: A new query parameter `fileName` has been added to be used as replacement of the default file name.
- README updated with new instructions for local deployment and testing.

## [0.0.1] - 2026.04.23

### Added

- Initial migration. CHANGELOG, README, and domain layer.

[0.0.3]: https://github.com/camiloapaezg/capg-cv-backend
[0.0.2]: https://github.com/camiloapaezg/capg-cv-backend/commit/401bf78f3ce1fea21dd30fe7b5762e70d5b599a2
[0.0.1]: https://github.com/camiloapaezg/capg-cv-backend/commit/0c5727dfb90e1640ec4b9a2a1f73504d63f86e62
