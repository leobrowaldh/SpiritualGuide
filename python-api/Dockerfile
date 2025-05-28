FROM python:3.10-slim

WORKDIR /app
COPY main.py .

RUN pip install fastapi uvicorn sentence-transformers torch

# Pre-download the model (will be cached in image layer)
RUN python3 -c "from sentence_transformers import SentenceTransformer; SentenceTransformer('intfloat/multilingual-e5-large')"

EXPOSE 8000
CMD ["uvicorn", "main:app", "--host", "0.0.0.0", "--port", "8000"]

