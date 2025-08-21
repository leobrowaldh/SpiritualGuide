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