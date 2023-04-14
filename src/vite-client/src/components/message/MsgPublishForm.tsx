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
import {
  ChangeEvent,
  Dispatch,
  RefObject,
  SetStateAction,
  useEffect,
} from "react";
import { IHeaderProps, SubjectDropDown } from "components";

function MsgPublishForm(props: {
  subjectInputRef: RefObject<HTMLSelectElement>;
  payloadInputRef: RefObject<HTMLInputElement>;
  buttonDisable: boolean;
  toggleButtonDisable: Dispatch<SetStateAction<boolean>>;
  headerList: IHeaderProps[];
  setHeaderList: Dispatch<SetStateAction<IHeaderProps[]>>;
}) {
  useEffect(() => {
    validateAllInputs();
  }, [props.headerList]);

  function isAscii(str: string) {
    return /\S/.test(str) && /^[\x00-\x7F]+$/.test(str) ? true : false;
  }

  function validateHeaders() {
    return props.headerList.every((headerPair: IHeaderProps) => {
      return isAscii(headerPair.name) && isAscii(headerPair.value)
        ? true
        : false;
    })
      ? true
      : false;
  }

  function validateInputs() {
    return isAscii(props.payloadInputRef.current!.value) &&
      isAscii(props.subjectInputRef.current!.value)
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

  function handleHeaderChange(e: ChangeEvent<HTMLInputElement>, index: number) {
    const tempList = [...props.headerList];
    const { id, value }: { id: string; value: string } = e.target;
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
      </FormControl>
      <FormControl>
        <FormLabel mb={0} mt={3}>
          Headers
        </FormLabel>
        {props.headerList.map((headerPair: IHeaderProps, index: number) => {
          return (
            <Box key={index} w={"100%"}>
              <HStack align={"end"}>
                <VStack align={"start"}>
                  {index === 0 && <FormHelperText>Name</FormHelperText>}
                  <Input
                    type={"text"}
                    id="name"
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
                    type={"text"}
                    id="value"
                    value={headerPair.value}
                    width={"100%"}
                    onChange={(e) => {
                      handleHeaderChange(e, index);
                      validateAllInputs();
                    }}
                    placeholder={"Value..."}
                  />
                </VStack>

                <IconButton
                  aria-label="Remove header"
                  bg={"inherit"}
                  onClick={() => {
                    handleHeaderRemove(index);
                    validateAllInputs();
                  }}
                  icon={<FiMinusCircle />}
                ></IconButton>
              </HStack>
            </Box>
          );
        })}
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
      </FormControl>
      <FormControl isRequired>
        <FormLabel mt={3}>Payload</FormLabel>
        <Input
          mb={3}
          mt={0}
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
