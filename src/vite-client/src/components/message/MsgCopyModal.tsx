import { CopyIcon } from "@chakra-ui/icons";
import {
  Button,
  IconButton,
  Modal,
  ModalBody,
  ModalCloseButton,
  ModalContent,
  ModalFooter,
  ModalHeader,
  ModalOverlay,
  useDisclosure,
} from "@chakra-ui/react";
import { ActionConfirmation, MsgCopyForm, MsgContext } from "components";
import { useContext, useRef, useState } from "react";

function MsgCopyModal() {
  const { isOpen, onOpen, onClose } = useDisclosure();
  const {
    isOpen: isOpenAC,
    onOpen: onOpenAC,
    onClose: onCloseAC,
  } = useDisclosure();
  const subjectInputRef = useRef<any>(null);
  const [buttonDisable, toggleButtonDisable] = useState<boolean>(true);
  const currentMsgContext = useContext(MsgContext);

  function copyMessage() {
    const msg = currentMsgContext?.currentMsg;
    if (msg) {
      let url = "/api/copyMessage";
      let requestMethod = "POST";
      let bodyContent;

      if (
        !process.env.NODE_ENV ||
        process.env.NODE_ENV === "development+json-server"
      ) {
        url += "/" + msg.sequenceNumber;
        requestMethod = "PUT";
        bodyContent = {
          id: msg.sequenceNumber,
          sequenceNumber: msg.sequenceNumber,
          timestamp: new Date(),
          stream: msg.stream,
          subject: subjectInputRef.current.value,
          headers: [],
          payload: "",
        };
      } else {
        bodyContent = {
          sequenceNumber: msg.sequenceNumber,
          stream: msg.stream,
          subject: subjectInputRef.current.value,
        };
      }

      fetch(url, {
        method: requestMethod,
        headers: {
          Accept: "application/json",
          "Content-Type": "application/json",
        },
        body: JSON.stringify(bodyContent),
      }).then((res) => {
        if (!res.ok) {
          alert(
            "An error occurred while copying the message: " + res.statusText
          );
        }
      });
    }
  }

  return (
    <>
      <IconButton
        mt={0.4}
        size={"md"}
        aria-label="Copy a message"
        onClick={() => {
          onOpen();
          toggleButtonDisable(true);
        }}
        icon={<CopyIcon />}
      />
      <Modal isOpen={isOpen} onClose={onClose} isCentered={true}>
        <ModalOverlay />
        <ModalContent maxW={"600px"}>
          <ModalHeader>Copy message</ModalHeader>
          <ModalCloseButton />
          <ModalBody>
            <MsgCopyForm
              content={""}
              subjectInputRef={subjectInputRef}
              buttonDisable={buttonDisable}
              toggleButtonDisable={toggleButtonDisable}
            />
          </ModalBody>
          <ModalFooter>
            <Button
              onClick={onOpenAC}
              mr={2}
              colorScheme="blue"
              isDisabled={buttonDisable}
            >
              Copy
            </Button>

            <ActionConfirmation
              action={copyMessage}
              isOpen={isOpenAC}
              onClose={() => {
                onClose();
                onCloseAC();
              }}
            />
          </ModalFooter>
        </ModalContent>
      </Modal>
    </>
  );
}

export { MsgCopyModal };
