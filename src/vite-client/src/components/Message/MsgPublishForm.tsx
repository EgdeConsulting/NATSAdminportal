import {
  Input,
  Button,
  FormControl,
  FormLabel,
  FormHelperText,
  useDisclosure,
} from "@chakra-ui/react";
import { useRef, useState } from "react";
import { ActionConfirmation, SubjectDropDown } from "components";

function MsgPublishForm() {
  const subjectInputRef = useRef<any>(null);
  const headerInputRef = useRef<any>(null);
  const payloadInputRef = useRef<any>(null);
  const [buttonDisable, toggleButtonDisable] = useState<boolean>(true);
  const { isOpen, onOpen, onClose } = useDisclosure();

  function postNewMessage() {
    fetch("/api/publishFullMessage", {
      method: "POST",
      headers: {
        Accept: "application/json",
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        subject: subjectInputRef.current.value,
        headers: headerInputRef.current.value,
        payload: payloadInputRef.current.value,
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

  function isAscii(str: string) {
    if (/\S/.test(str) && /^[\x00-\x7F]+$/.test(str)) {
      return true;
    } else {
      return false;
    }
  }

  function checkInputs() {
    if (
      payloadInputRef.current.value != "" &&
      isAscii(payloadInputRef.current.value) &&
      headerInputRef.current.value != "" &&
      isAscii(headerInputRef.current.value) &&
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
        <SubjectDropDown
          subjectInputRef={subjectInputRef}
          checkInputs={checkInputs}
        />
        <FormHelperText>
          Choose the subject you want to post your message to
        </FormHelperText>

        <FormLabel mt={3}>Headers</FormLabel>
        <Input
          type={"text"}
          ref={headerInputRef}
          width={"100%"}
          onChange={() => {
            checkInputs();
          }}
          placeholder={"Headers..."}
        />

        <FormLabel mt={3}>Payload</FormLabel>
        <Input
          mb={5}
          type={"text"}
          width={"100%"}
          onChange={() => {
            checkInputs();
          }}
          ref={payloadInputRef}
          placeholder={"Enter your message..."}
        />
      </FormControl>
      <Button
        mb={2}
        isDisabled={buttonDisable}
        colorScheme="blue"
        onClick={onOpen}
      >
        Publish
      </Button>
      <ActionConfirmation
        action={postNewMessage}
        buttonDisable={buttonDisable}
        toggleButtonDisable={toggleButtonDisable}
        onClose={onClose}
        isOpen={isOpen}
        buttonText={"Publish"}
        alertHeader={"Publish Message"}
      />
    </>
  );
}

export { MsgPublishForm };
