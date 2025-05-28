from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
from sentence_transformers import SentenceTransformer
import torch

app = FastAPI()

model = SentenceTransformer("intfloat/multilingual-e5-large")

class EmbedRequest(BaseModel):
    inputs: list[str]

class EmbedResponse(BaseModel):
    embeddings: list[list[float]]

@app.post("/embed", response_model=EmbedResponse)
def embed(request: EmbedRequest):
    try:
        with torch.no_grad():
            embs = model.encode(request.inputs, convert_to_numpy=True)
        return {"embeddings": embs.tolist()}
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))
