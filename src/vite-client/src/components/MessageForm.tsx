import {
  Select,
  Input,
  Button,
  FormControl,
  FormLabel,
  FormErrorMessage,
  FormHelperText,
} from "@chakra-ui/react";
import { useRef, useEffect, useState } from "react";

function MessageForm() {
  const subjectInputRef = useRef<any>(null);
  const headerInputRef = useRef<any>(null);
  const payloadInputRef = useRef<any>(null);
  const [subjects, setSubjects] = useState<[]>([]);
  const [buttonDisable, toggleButtonDisable] = useState<boolean>(true);

  useEffect(() => {
    getSubjects();
  }, [subjects.length != 0]);

  function getSubjects() {
    fetch("/SubjectNames")
      .then((res) => res.json())
      .then((data) => {
        setSubjects(data); //Should consider removing stars from subjects?
      });
  }

  function postNewMessage() {
    fetch("/PublishFullMessage", {
      method: "POST",
      headers: {
        Accept: "application/json",
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        Subject: subjectInputRef.current.value,
        Headers: headerInputRef.current.value,
        Payload: payloadInputRef.current.value,
      }),
    }).then((res) => {
      if (res.ok) {
        //This doesnt actually check if the nats server has received the message... Need to find a way to do this
        //Create a subscriber based on teh same subject that replies? Hard to do...
        subjectInputRef.current.value = "";
        headerInputRef.current.value = "";
        payloadInputRef.current.value = "";
      } else {
        alert("Network error: " + res.statusText);
      }
    });
  }

  function checkInputs() {
    if (
      payloadInputRef.current.value != "" &&
      headerInputRef.current.value != "" &&
      subjectInputRef.current.value != ""
    ) {
      toggleButtonDisable(false);
    } else {
      toggleButtonDisable(true);
    }
  }
  return (
    <>
      <FormControl isRequired>
        <FormLabel>Subject</FormLabel>
        <Select
          ref={subjectInputRef}
          placeholder="Select a subject"
          onChange={checkInputs}
        >
          {subjects.map((subject: any, index: number) => {
            return <option key={index}>{subject["name"]}</option>;
          })}
        </Select>
        <FormHelperText>
          Choose the subject you want to post your message to
        </FormHelperText>

        <FormLabel marginTop={3}>Headers</FormLabel>
        <Input
          type={"text"}
          ref={headerInputRef}
          width={"100%"}
          onChange={checkInputs}
          placeholder={"Headers..."}
        />

        <FormLabel marginTop={3}>Payload</FormLabel>
        <Input
          marginBottom={5}
          type={"text"}
          width={"100%"}
          onChange={checkInputs}
          ref={payloadInputRef}
          placeholder={"Enter your message..."}
        />
      </FormControl>
      <Button
        onClick={postNewMessage}
        isDisabled={buttonDisable}
        marginRight={2}
        colorScheme="blue"
      >
        Publish
      </Button>
    </>
  );
}

export { MessageForm };
