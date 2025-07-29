type InputFieldProps = {
  onAsk: (input: string) => void;
};

export default function InputField({ onAsk }: InputFieldProps) {

  function handleSubmit(e: React.FormEvent<HTMLFormElement>) {
    e.preventDefault(); // prevent page reload
    const formData = new FormData(e.currentTarget);
    const input = formData.get('userInput') as string;
    onAsk(input);
  }

  return (
    <form method='post' onSubmit={handleSubmit} className="flex flex-row h-full shadow-lg rounded-xl">
      <input 
        name="userInput"
        className="flex-1 p-2 rounded-l" 
        placeholder="Ask anything..." 
        defaultValue=""
      />
      <button type='submit' className="bg-cyan-600 dark:bg-blue-950 hover:bg-sky-800 active:bg-indigo-400 px-4 rounded-r shadow-lg">
        Ask
      </button>
    </form>
  )
}