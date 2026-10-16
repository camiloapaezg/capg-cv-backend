# CAPG CV Backend API

## Purpose

The CAPG CV Backend API manages CV/resume data for users, including personal and general profile details, work experience, formal education, certification/training records, publications, and associated files such as profile photos, certificates, and work-experience attachments.

## API endpoints

All routes below are relative to the API root shown in each group. GUID route parameters must be valid GUIDs. JSON request bodies use the DTOs and fields listed below; fields marked `?` are nullable/optional at the DTO level. Application validators may impose additional constraints.

### Users

**Route group:** `/api/v1/users`

| Method   | Endpoint             | Input (path/body)                         | Success response                                                                  |
| -------- | -------------------- | ----------------------------------------- | --------------------------------------------------------------------------------- |
| `GET`    | `/api/v1/users/`     | None                                      | `200 OK` — `List<User>`                                                           |
| `GET`    | `/api/v1/users/{id}` | Path: `id` (`Guid`)                       | `200 OK` — `User`                                                                 |
| `POST`   | `/api/v1/users/`     | JSON body: `UserDto` (see below)          | `201 Created` — created `User`; `Location` points to `/api/v1/users/{newUser.Id}` |
| `PUT`    | `/api/v1/users/{id}` | Path: `id` (`Guid`); JSON body: `UserDto` | `204 No Content`                                                                  |
| `DELETE` | `/api/v1/users/{id}` | Path: `id` (`Guid`)                       | `202 Accepted`; deletion of associated files is queued when files exist           |

**JSON body**

```json
{
  "firstName": "Ada",
  "lastName": "Lovelace",
  "emailAddress": "ada@example.com",
  "birthDate": "1815-12-10T00:00:00",
  "location": "London",
  "nationality": "British",
  "telephoneNumber": "+44 0000 000000"
}
```

Properties: `firstName` (string), `lastName` (string), `emailAddress` (string), `birthDate` (DateTime), `location` (string or null), `nationality` (string or null), `telephoneNumber` (string or null).

Creating a user also creates initial `GeneralDetails` and `PersonalDetails` records. Updating a user updates the user's name/email and the associated personal details from the same `UserDto`.

### Personal details

**Route group:** `/api/v1/users/{userId}/personal`

| Method | Endpoint                           | Input (path/body)                                        | Success response             |
| ------ | ---------------------------------- | -------------------------------------------------------- | ---------------------------- |
| `GET`  | `/api/v1/users/{userId}/personal/` | Path: `userId` (`Guid`)                                  | `200 OK` — `PersonalDetails` |
| `PUT`  | `/api/v1/users/{userId}/personal/` | Path: `userId` (`Guid`); JSON body: `PersonalDetailsDto` | `204 No Content`             |

**JSON body**

```json
{
  "birthDate": "1815-12-10T00:00:00",
  "location": "London",
  "nationality": "British",
  "telephoneNumber": "+44 0000 000000"
}
```

Properties: `birthDate` (DateTime), `location` (string or null), `nationality` (string or null), `telephoneNumber` (string or null).

### General details

**Route group:** `/api/v1/users/{userId}/general`

| Method | Endpoint                               | Input (path/body)                                          | Success response                         |
| ------ | -------------------------------------- | ---------------------------------------------------------- | ---------------------------------------- |
| `GET`  | `/api/v1/users/{userId}/general/`      | Path: `userId` (`Guid`)                                    | `200 OK` — `GeneralDetails`              |
| `PUT`  | `/api/v1/users/{userId}/general/`      | Path: `userId` (`Guid`); JSON body: `GeneralDetailsDto`    | `204 No Content`                         |
| `GET`  | `/api/v1/users/{userId}/general/photo` | Path: `userId` (`Guid`)                                    | File stream (`application/octet-stream`) |
| `POST` | `/api/v1/users/{userId}/general/photo` | Path: `userId` (`Guid`); multipart form file field: `file` | `202 Accepted`                           |

**JSON body**

```json
{
  "title": "Software Engineer",
  "description": "Professional profile summary",
  "technicalSkills": "C#, TypeScript, PostgreSQL",
  "languages": "English, German"
}
```

Properties: `title` (string), `description` (string or null), `technicalSkills` (string or null), `languages` (string or null).

Photo upload uses `multipart/form-data` with a file field named `file`; it does not use a JSON body.

### Work experience

**Route group:** `/api/v1/users/{userId}/experience`

| Method   | Endpoint                                      | Input (path/body)                                                | Success response                         |
| -------- | --------------------------------------------- | ---------------------------------------------------------------- | ---------------------------------------- |
| `GET`    | `/api/v1/users/{userId}/experience/`          | Path: `userId` (`Guid`)                                          | `200 OK` — `List<WorkExperience>`        |
| `GET`    | `/api/v1/users/{userId}/experience/{id}`      | Path: `userId`, `id` (`Guid`)                                    | `200 OK` — `WorkExperience`              |
| `POST`   | `/api/v1/users/{userId}/experience/`          | Path: `userId` (`Guid`); JSON body: `WorkExperienceDto`          | `201 Created` — created `WorkExperience` |
| `PUT`    | `/api/v1/users/{userId}/experience/{id}`      | Path: `userId`, `id` (`Guid`); JSON body: `WorkExperienceDto`    | `204 No Content`                         |
| `DELETE` | `/api/v1/users/{userId}/experience/{id}`      | Path: `userId`, `id` (`Guid`)                                    | `204 No Content`                         |
| `GET`    | `/api/v1/users/{userId}/experience/{id}/file` | Path: `userId`, `id` (`Guid`)                                    | File stream (`application/octet-stream`) |
| `POST`   | `/api/v1/users/{userId}/experience/{id}/file` | Path: `userId`, `id` (`Guid`); multipart form file field: `file` | `202 Accepted`                           |

**JSON body**

```json
{
  "company": "Example Ltd",
  "jobTitle": "Software Engineer",
  "location": "Augsburg, Germany",
  "description": "Developed backend and frontend software",
  "from": "2022-01-01T00:00:00",
  "until": null,
  "contactName": null,
  "contactNumber": null
}
```

Properties: `company` (string), `jobTitle` (string), `location` (string), `description` (string), `from` (DateTime), `until` (DateTime or null), `contactName` (string or null), `contactNumber` (string or null).

File download requires the experience entry to exist, belong to the supplied `userId`, and have an attached file. Upload accepts `multipart/form-data` with a file field named `file`.

### Formal education

**Route group:** `/api/v1/users/{userId}/education`

| Method   | Endpoint                                | Input (path/body)                                              | Success response                          |
| -------- | --------------------------------------- | -------------------------------------------------------------- | ----------------------------------------- |
| `GET`    | `/api/v1/users/{userId}/education/`     | Path: `userId` (`Guid`)                                        | `200 OK` — `List<FormalEducation>`        |
| `GET`    | `/api/v1/users/{userId}/education/{id}` | Path: `userId`, `id` (`Guid`)                                  | `200 OK` — `FormalEducation`              |
| `POST`   | `/api/v1/users/{userId}/education/`     | Path: `userId` (`Guid`); JSON body: `FormalEducationDto`       | `201 Created` — created `FormalEducation` |
| `PUT`    | `/api/v1/users/{userId}/education/{id}` | Path: `userId`, `id` (`Guid`); JSON body: `FormalEducationDto` | `204 No Content`                          |
| `DELETE` | `/api/v1/users/{userId}/education/{id}` | Path: `userId`, `id` (`Guid`)                                  | `204 No Content`                          |

**JSON body**

```json
{
  "degree": "MSc Computer Science",
  "institution": "Example University",
  "startDate": "2020-09",
  "endDate": "2022-06",
  "description": "Specialized in software engineering"
}
```

Properties: `degree` (string), `institution` (string), `startDate` (string), `endDate` (string or null), `description` (string or null).

### Certification and training

**Route group:** `/api/v1/users/{userId}/certificationtraining`

| Method   | Endpoint                                                 | Input (path/body)                                                    | Success response                                |
| -------- | -------------------------------------------------------- | -------------------------------------------------------------------- | ----------------------------------------------- |
| `GET`    | `/api/v1/users/{userId}/certificationtraining/`          | Path: `userId` (`Guid`)                                              | `200 OK` — `List<CertificationTraining>`        |
| `GET`    | `/api/v1/users/{userId}/certificationtraining/{id}`      | Path: `userId`, `id` (`Guid`)                                        | `200 OK` — `CertificationTraining`              |
| `POST`   | `/api/v1/users/{userId}/certificationtraining/`          | Path: `userId` (`Guid`); JSON body: `CertificationTrainingDto`       | `201 Created` — created `CertificationTraining` |
| `PUT`    | `/api/v1/users/{userId}/certificationtraining/{id}`      | Path: `userId`, `id` (`Guid`); JSON body: `CertificationTrainingDto` | `204 No Content`                                |
| `DELETE` | `/api/v1/users/{userId}/certificationtraining/{id}`      | Path: `userId`, `id` (`Guid`)                                        | `204 No Content`                                |
| `GET`    | `/api/v1/users/{userId}/certificationtraining/{id}/file` | Path: `userId`, `id` (`Guid`)                                        | File stream (`application/octet-stream`)        |
| `POST`   | `/api/v1/users/{userId}/certificationtraining/{id}/file` | Path: `userId`, `id` (`Guid`); multipart form file field: `file`     | `202 Accepted`                                  |

**JSON body**

```json
{
  "institution": "Example Academy",
  "title": "Cloud Certification",
  "finishedAt": "2025-03",
  "certificateNumber": null
}
```

Properties: `institution` (string), `title` (string), `finishedAt` (string), `certificateNumber` (string or null).

Certificate file upload uses `multipart/form-data` with a file field named `file`.

### Publications

**Route group:** `/api/v1/users/{userId}/publication`

| Method   | Endpoint                                  | Input (path/body)                                          | Success response                      |
| -------- | ----------------------------------------- | ---------------------------------------------------------- | ------------------------------------- |
| `GET`    | `/api/v1/users/{userId}/publication/`     | Path: `userId` (`Guid`)                                    | `200 OK` — `List<Publication>`        |
| `GET`    | `/api/v1/users/{userId}/publication/{id}` | Path: `userId`, `id` (`Guid`)                              | `200 OK` — `Publication`              |
| `POST`   | `/api/v1/users/{userId}/publication/`     | Path: `userId` (`Guid`); JSON body: `PublicationDto`       | `201 Created` — created `Publication` |
| `PUT`    | `/api/v1/users/{userId}/publication/{id}` | Path: `userId`, `id` (`Guid`); JSON body: `PublicationDto` | `204 No Content`                      |
| `DELETE` | `/api/v1/users/{userId}/publication/{id}` | Path: `userId`, `id` (`Guid`)                              | `204 No Content`                      |

**JSON body**

```json
{
  "title": "An Example Publication",
  "type": "Journal",
  "location": "Example Journal",
  "publishedAt": "2024-01",
  "doi": null
}
```

Properties: `title` (string), `type` (string), `location` (string), `publishedAt` (string), `doi` (string or null).

## Responses and validation

- Collection endpoints return JSON arrays of the corresponding domain entity.
- Single-item read endpoints return the corresponding domain entity.
- Create endpoints return `201 Created` and the created entity, except where the endpoint implementation indicates otherwise.
- Update endpoints return `204 No Content` on success.
- Delete endpoints for child records return `204 No Content`; deleting a user returns `202 Accepted`.
- DTO validation failures return `422 Unprocessable Entity` with formatted validation errors.
- Invalid or missing entities and user/entity ownership mismatches commonly return `400 Bad Request`.
- Internal operation failures may return `500 Internal Server Error` or a problem response with the downstream file-operation status.
- File upload endpoints accept `multipart/form-data` with a single `file` field; file downloads return a binary stream rather than JSON.
