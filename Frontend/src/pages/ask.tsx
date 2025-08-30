import { useState } from 'react';
import AnswerField from '../components/answerField';
import InputField from '../components/inputField';
import { IoMdSettings } from "react-icons/io";
import { protectedResources } from '../auth/authConfig';
import useFetchWithMsal from '../services/useFetchWithMsal';
import type { AskResponse } from '../models/responseModels';

export default function AskPage() {

  const { error, execute } = useFetchWithMsal({
        scopes: protectedResources.spiritualGuideAPI.scopes.write,
    });

  const [answer, setAnswer] = useState('');
  // const [teachers, setTeachers] = useState<number[]>([0,1,2,3,4,5]);

  console.log("Executing test API call...");
  execute("POST", "https://localhost:7123/ask", { text: "hello" });


  async function handleAsk(input: string) {
    console.log("Calling execute with endpoint:", `${protectedResources.spiritualGuideAPI.endpoint}/ask`);
    const response = await execute(
      "POST", 
      `${protectedResources.spiritualGuideAPI.endpoint}/ask`, 
      { question: input, }
    );

    if (response) {
      const askResponse = response as AskResponse;
      setAnswer(`${askResponse.quote} - ${askResponse.author}`);
    } else {
      setAnswer("No answer found");
    }
  }
  
  return (
    <div className="bg-gray-300 dark:bg-neutral-900 rounded-lg my-14 flex-1 flex flex-col p-6 gap-4 w-[90%] max-w-4xl mx-auto shadow-lg">
      <div className="h-14 bg-white dark:bg-neutral-800">
        <InputField onAsk={handleAsk} />
      </div>

      <div className="h-8 flex justify-start items-center">
        <IoMdSettings/>
      </div>

      <div className="bg-white dark:bg-neutral-800 rounded-lg flex-1 shadow-lg p-2">
        {error 
          ? <div>
              Something went wrong while fetching your answer. Please try again later or contact support.
            </div>
          : <AnswerField answer={answer} />}
      </div>

    </div>
  );
}