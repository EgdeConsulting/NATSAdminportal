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
      const queryString =
        "streamName=" +
        msg.stream +
        "&sequenceNumber=" +
        msg.sequenceNumber +
        "&newSubject=" +
        subjectInputRef.current.value;
      fetch("/api/copyMessage?" + queryString, {
        method: "POST",
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
