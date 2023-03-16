import {
  IconButton,
  Button,
  Modal,
  ModalOverlay,
  ModalContent,
  ModalHeader,
  ModalBody,
  ModalCloseButton,
  useDisclosure,
  ModalFooter,
  Tooltip,
} from "@chakra-ui/react";
import { ChatIcon } from "@chakra-ui/icons";
import { MsgPublishForm, ActionConfirmation } from "components";
import { Dispatch, SetStateAction, useRef, useState } from "react";

function MsgPublishModal() {
  const [buttonDisable, toggleButtonDisable] = useState<boolean>(true);
  const { isOpen, onOpen, onClose } = useDisclosure();
  const {
    isOpen: isOpenAC,
    onOpen: onOpenAC,
    onClose: onCloseAC,
  } = useDisclosure();

  const [headerList, setHeaderList] = useState<any[]>([
    { name: "", value: "" },
  ]);
  const subjectInputRef = useRef<any>(null);
  const payloadInputRef = useRef<any>(null);

  function postNewMessage() {
    fetch("/api/newMessage", {
      method: "POST",
      headers: {
        Accept: "application/json",
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        Subject: subjectInputRef.current.value,
        Headers: headerList,
        Payload: payloadInputRef.current.value,
      }),
    }).then((res) => {
      if (res.ok) {
        subjectInputRef.current.value = "";
        setHeaderList([{ name: "", value: "" }]);
        payloadInputRef.current.value = "";
      } else {
        alert("Network error: " + res.statusText);
      }
    });
  }

  return (
    <>
      <IconButton
        margin={2}
        size={"md"}
        onClick={() => {
          setHeaderList([{ name: "", value: "" }]);
          onOpen();
        }}
        aria-label="Publish a message"
        icon={<ChatIcon />}
      />

      <Modal size={"md"} isOpen={isOpen} onClose={onClose}>
        <ModalOverlay />
        <ModalContent>
          <ModalHeader>Publish message</ModalHeader>
          <ModalCloseButton />
          <ModalBody>
            <MsgPublishForm
              buttonDisable={buttonDisable}
              toggleButtonDisable={toggleButtonDisable}
              subjectInputRef={subjectInputRef}
              payloadInputRef={payloadInputRef}
              headerList={headerList}
              setHeaderList={setHeaderList}
            />
          </ModalBody>
          <ModalFooter>
            <Tooltip
              isDisabled={!buttonDisable}
              hasArrow
              label="Select subject, provide at least 1 header and provide payload. ASCII characters only"
              aria-label="Reqs for publish"
            >
              <Button
                mb={2}
                isDisabled={buttonDisable}
                colorScheme="blue"
                onClick={onOpenAC}
              >
                Publish
              </Button>
            </Tooltip>
            <Button
              mb={2}
              ml={2}
              variant="ghost"
              onClick={() => {
                setHeaderList([{ name: "", value: "" }]);
                onClose();
              }}
            >
              Close
            </Button>
            <ActionConfirmation
              action={postNewMessage}
              onClose={() => {
                onCloseAC();
              }}
              isOpen={isOpenAC}
            />
          </ModalFooter>
        </ModalContent>
      </Modal>
    </>
  );
}

export { MsgPublishModal };
