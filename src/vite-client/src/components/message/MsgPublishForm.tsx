import {
  Input,
  Button,
  FormControl,
  FormLabel,
  FormHelperText,
  HStack,
  VStack,
  IconButton,
  Box,
} from "@chakra-ui/react";
import { FiPlusCircle, FiMinusCircle } from "react-icons/fi";
import { Dispatch, SetStateAction, useEffect, useRef, useState } from "react";
import { SubjectDropDown } from "components";

function MsgPublishForm(props: {
  subjectInputRef: any;
  payloadInputRef: any;
  buttonDisable: boolean;
  toggleButtonDisable: Dispatch<SetStateAction<boolean>>;
  headerList: any[];
  setHeaderList: Dispatch<SetStateAction<any[]>>;
}) {
  useEffect(() => {
    validateAllInputs();
  }, [props.headerList]);

  function isAscii(str: string) {
    return /\S/.test(str) && /^[\x00-\x7F]+$/.test(str) ? true : false;
  }

  function validateHeaders() {
    return props.headerList.every((headerPair: any) => {
      return isAscii(headerPair.name) && isAscii(headerPair.value)
        ? true
        : false;
    })
      ? true
      : false;
  }

  function validateInputs() {
    return isAscii(props.payloadInputRef.current.value) &&
      isAscii(props.subjectInputRef.current.value)
      ? true
      : false;
  }

  function validateAllInputs() {
    validateInputs() && validateHeaders()
      ? props.toggleButtonDisable(false)
      : props.toggleButtonDisable(true);
  }

  function handleHeaderAdd() {
    props.setHeaderList([...props.headerList, { name: "", value: "" }]);
  }

  function handleHeaderRemove(index: number) {
    const tempList = [...props.headerList];
    tempList.splice(index, 1);
    props.setHeaderList(tempList);
  }

  function handleHeaderChange(e: any, index: number) {
    const tempList = [...props.headerList];
    const { id, value } = e.target;
    tempList[index][id] = value;
    props.setHeaderList(tempList);
  }
  return (
    <>
      <FormControl isRequired>
        <FormLabel>Subject</FormLabel>
        <SubjectDropDown
          subjectInputRef={props.subjectInputRef}
          validateAllInputs={validateAllInputs}
        />
        <FormHelperText>
          Choose the subject you want to post your message to
        </FormHelperText>
        <FormLabel mt={3}>Headers</FormLabel>
        {props.headerList.map((headerPair: any, index: number) => {
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
                {props.headerList.length > 1 && (
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
              {props.headerList.length - 1 === index && (
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
          ref={props.payloadInputRef}
          placeholder={"Enter your message..."}
        />
      </FormControl>
    </>
  );
}

export { MsgPublishForm };
