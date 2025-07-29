import { useState } from 'react';
import AnswerField from '../components/answerField';
import InputField from '../components/inputField';
import { askQuestion, type AskResponse } from '../services/apiService'
import { IoMdSettings } from "react-icons/io";

export default function AskPage() {
  const [answer, setAnswer] = useState('');
  // const [teachers, setTeachers] = useState<number[]>([0,1,2,3,4,5]);

  async function handleAsk(input: string) {
    
    const response: AskResponse = await askQuestion(input);

    setAnswer(`${response.quote} - ${response.author}` || "No answer found");

  }

  return (
    <div className="bg-gray-300 dark:bg-neutral-900 rounded-lg my-14 flex-1 flex flex-col p-6 gap-4 w-[90%] max-w-4xl mx-auto shadow-lg">
      <div className="h-14 bg-white dark:bg-neutral-800">
        <InputField onAsk={handleAsk} />
      </div>

      <div className="h-8 flex justify-start items-center">
        <IoMdSettings/>
      </div>

      <div className="bg-white dark:bg-neutral-800 rounded-lg flex-1 shadow-lg">
        <AnswerField answer={answer} />
      </div>
    </div>
  );
}