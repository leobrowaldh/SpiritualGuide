// src/services/apiService.ts
// Service for making API calls to the FunctionApp backend

const functionKey = import.meta.env.VITE_FUNCTION_KEY;

const API_BASE_URL =
  import.meta.env.MODE === 'development'
    ? 'http://localhost:7031/api'
    : import.meta.env.VITE_API_BASE_URL;



export type AskResponse = { quote: string; author: string; similarity: number };
export type TableData = {
  QuoteString: string;
  OpenAi3SEmbeddingJson: string;
  OpenAi3LEmbeddingJson: string;
  HFe5bEmbeddingJson: string;
  HFe5lEmbeddingJson: string;
  HFminiLMEmbeddingJson: string;
  HFdistiluseEmbeddingJson: string;
  PartitionKey: string;
  RowKey: string;
  Timestamp?: string;
  ETag?: string;
};
export type AddQuoteResponse = TableData[];

async function fetchApi<T>(url: string, body: any): Promise<T> {
  const response = await fetch(url, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(body),
  });
  if (!response.ok) {
    throw new Error('API request failed');
  }
  return response.json() as Promise<T>;
}

export function askQuestion(question: string): Promise<AskResponse> {
  return fetchApi<AskResponse>(`${API_BASE_URL}/Ask`, { question });
}

//this one wont work, need master key, i wont access it from this ui.
export function addQuote(quote: string): Promise<AddQuoteResponse> {
  return fetchApi<AddQuoteResponse>(`${API_BASE_URL}/AddQuote`, { quote });
}
