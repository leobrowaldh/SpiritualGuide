type AnswerFieldProps = {
  answer: string;
};

export default function AnswerField({ answer }: AnswerFieldProps) {
  return (
    <div className="p-4 shadow-lg relative w-full min-h-100">
      {answer ? (
        <p>{answer}</p>
      ) : (
        <img
          src="/nature.jpg"
          alt="meditating zen master"
          className="absolute inset-0 w-full h-full object-fill rounded-lg"
        />
      )}
    </div>
  );
}