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
import { ActionConfirmation, MsgCopyForm } from "components";
import { useRef, useState } from "react";

function MsgCopyModal(props: { content: any }) {
  const { isOpen, onOpen, onClose } = useDisclosure();
  const {
    isOpen: isOpenAC,
    onOpen: onOpenAC,
    onClose: onCloseAC,
  } = useDisclosure();

  const subjectInputRef = useRef<any>(null);
  const [buttonDisable, toggleButtonDisable] = useState<boolean>(true);

  function copyMessage() {
    const queryString =
      "streamName=" +
      props.content["stream"] +
      "&sequenceNumber=" +
      props.content["sequenceNumber"] +
      "&newSubject=" +
      subjectInputRef.current.value;
    fetch("/api/copyMessage?" + queryString, {
      method: "POST",
    });
  }

  return (
    <>
      <IconButton
        mt={2}
        mr={2}
        ml={2}
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
        <ModalContent>
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
              buttonText={"Copy"}
              alertHeader={"Copy message"}
            />
          </ModalFooter>
        </ModalContent>
      </Modal>
    </>
  );
}

export { MsgCopyModal };
