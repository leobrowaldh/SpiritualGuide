# SpiritualGuide

## Description

This application receives user questions or thoughts, transform them into vector embeddings that are compared to a database of embedded vectors from spiritual quotes and return the best semantic match through cosine similarity comparison. In this way it correlates the meaning the user is asking for with the meaning contained in the quotes. 

The api can communicate with the OpenAi api for embeddings, and a HuggingFace model is also included as a docker file with a Python FastApi, so the api can change between these two, or can easily be adapted to use any other model. 

In future versions audio and/or video streaming will replace the simple quotes, and embedding of the speech to text user input is then compared to the embeddings of the transcriptions of the audio/video prechunked into convenient segments with spiritual context. For this segmentation and storage of embeddings a media processing application will be developed.

![Architecture Diagram](./docs/spiritualguide%20diagram.png)

## API
The [api](./api/) itself is a ASP.NET minimal api with Entra External Id as authentication and authorization layer.

## Frontend
The [Frontend](./Frontend/) is a Vite React application written in Typescript with MSAL for Auth.

## Python API
The [Python API](./python-api/) Is a tiny Fast API used to comunicate with the Hugging Face Model for vector embeddings.


## Tech Stack
- ASP.NET Minimal API
- Entra External ID (OAuth2 / OIDC)
- React + Vite + TypeScript
- MSAL.js for authentication
- Python FastAPI (HuggingFace embeddings)
- OpenAI API (OpenAI embeddings)
- Docker
- Azure Storage Account - Table Storage (vector DB)
