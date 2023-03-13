import {
  Input,
  Button,
  FormControl,
  FormLabel,
  FormHelperText,
  useDisclosure,
  HStack,
  VStack,
  IconButton,
  Box,
  Tooltip,
} from "@chakra-ui/react";
import { FiPlusCircle, FiMinusCircle } from "react-icons/fi";
import { useEffect, useRef, useState } from "react";
import { ActionConfirmation, SubjectDropDown } from "components";

function MsgPublishForm() {
  const subjectInputRef = useRef<any>(null);

  const payloadInputRef = useRef<any>(null);
  const [buttonDisable, toggleButtonDisable] = useState<boolean>(true);
  const [headerList, setHeaderList] = useState<any[]>([
    { name: "", value: "" },
  ]);
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
        headers: "test", //JSON.stringify(headerList),
        payload: payloadInputRef.current.value,
      }),
    }).then((res) => {
      if (res.ok) {
        //This doesnt actually check if the nats server has received the message... Need to find a way to do this
        //Create a subscriber based on teh same subject that replies? Hard to do...
        subjectInputRef.current.value = "";
        setHeaderList([{ name: "", value: "" }]); // THIS NEEDS TO BE TESTED!!!!!!!!
        payloadInputRef.current.value = "";
      } else {
        alert("Network error: " + res.statusText);
      }
    });
  }

  useEffect(() => {
    validateAllInputs();
  }, [headerList]);

  function isAscii(str: string) {
    return /\S/.test(str) && /^[\x00-\x7F]+$/.test(str) ? true : false;
  }

  function validateHeaders() {
    return headerList.every((headerPair: any) => {
      return isAscii(headerPair.name) && isAscii(headerPair.value)
        ? true
        : false;
    })
      ? true
      : false;
  }

  function validateInputs() {
    return isAscii(payloadInputRef.current.value) &&
      isAscii(subjectInputRef.current.value)
      ? true
      : false;
  }

  function validateAllInputs() {
    validateInputs() && validateHeaders()
      ? toggleButtonDisable(false)
      : toggleButtonDisable(true);
  }

  function handleHeaderAdd() {
    setHeaderList([...headerList, { name: "", value: "" }]);
  }

  function handleHeaderRemove(index: number) {
    const tempList = [...headerList];
    tempList.splice(index, 1);
    setHeaderList(tempList);
  }

  function handleHeaderChange(e: any, index: number) {
    const tempList = [...headerList];
    const { id, value } = e.target;
    tempList[index][id] = value;
    setHeaderList(tempList);
  }
  return (
    <>
      <FormControl isRequired>
        <FormLabel>Subject</FormLabel>
        <SubjectDropDown
          subjectInputRef={subjectInputRef}
          validateAllInputs={validateAllInputs}
        />
        <FormHelperText>
          Choose the subject you want to post your message to
        </FormHelperText>
        <FormLabel mt={3}>Headers</FormLabel>
        {headerList.map((headerPair: any, index: number) => {
          return (
            <Box key={index} w={"100%"}>
              <HStack align={"end"}>
                <VStack align={"start"}>
                  {index === 0 && <FormHelperText>Name</FormHelperText>}
                  <Input
                    id="name"
                    type={"text"}
                    value={headerPair.name}
                    width={"100%"}
                    onChange={(e) => {
                      handleHeaderChange(e, index);
                      validateAllInputs();
                    }}
                    placeholder={"Name..."}
                  />
                </VStack>
                <VStack align={"start"}>
                  {index === 0 && <FormHelperText>Value</FormHelperText>}
                  <Input
                    id="value"
                    type={"text"}
                    value={headerPair.value}
                    width={"100%"}
                    onChange={(e) => {
                      handleHeaderChange(e, index);
                      validateAllInputs();
                    }}
                    placeholder={"Value..."}
                  />
                </VStack>
                {headerList.length > 1 && (
                  <IconButton
                    aria-label="Remove header"
                    bg={"inherit"}
                    onClick={() => {
                      handleHeaderRemove(index);
                      validateAllInputs();
                    }}
                    icon={<FiMinusCircle />}
                  ></IconButton>
                )}
              </HStack>
              {headerList.length - 1 === index && (
                <Button
                  leftIcon={<FiPlusCircle />}
                  aria-label="Add more headers"
                  mt={1}
                  bg={"inherit"}
                  onClick={() => {
                    handleHeaderAdd();
                    validateAllInputs();
                  }}
                >
                  Add header
                </Button>
              )}
            </Box>
          );
        })}

        <FormLabel mt={3}>Payload</FormLabel>
        <Input
          mb={5}
          type={"text"}
          width={"100%"}
          onChange={() => {
            validateAllInputs();
          }}
          ref={payloadInputRef}
          placeholder={"Enter your message..."}
        />
      </FormControl>
      <Tooltip
        isDisabled={!buttonDisable}
        hasArrow
        label="Select subject, provide at least 1 header and provide payload. ASCII characters only"
        aria-label="Reqs for publish"
      >
        <Button
          mb={2}
          mt={4}
          isDisabled={buttonDisable}
          colorScheme="blue"
          onClick={onOpen}
        >
          Publish
        </Button>
      </Tooltip>
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
