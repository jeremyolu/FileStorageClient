# FilStorageeClient

## Overview
`FileClient` is a simple .NET client library for accessing and managing files either on the local disk or in Azure Blob Storage. It provides easy-to-use asynchronous methods for retrieving, checking existence, and deleting files across different storage types.

---

## Features
- Support for multiple file types:
  - Local disk files
  - Azure Blob Storage files
- Async file retrieval with content metadata (filename, content type, raw bytes, Base64 content)
- Check if a file exists asynchronously
- Delete files asynchronously
- Optional callback delegate for additional processing when a file is found (e.g., scanning, logging)
- Automatic MIME type detection for common file extensions
- Conversion utilities for Base64 encoding/decoding of strings

---

## `StorageClient` class - Core Methods

### `GetFileAsync`
- Retrieves a file from disk or Azure Blob Storage.
- Returns `StorageFileResponse` with file content and metadata.
- Accepts an optional delegate to run custom code when the file is found (e.g., scanning, logging).

### `FileExistsAsync`
- Checks asynchronously if a file exists on disk or in Azure Blob Storage.
- Returns a boolean indicating existence.

### `DeleteFileAsync`
- Deletes a file from disk or Azure Blob Storage asynchronously.
- Returns a `FileResponse` indicating success or failure.

---

## How to Use

### 1. Registering with Dependency Injection (recommended)
