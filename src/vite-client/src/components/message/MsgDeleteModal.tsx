import {
  IconButton,
  Button,
  Modal,
  ModalOverlay,
  ModalContent,
  ModalHeader,
  ModalFooter,
  ModalBody,
  ModalCloseButton,
  useDisclosure,
} from "@chakra-ui/react";
import { DeleteIcon } from "@chakra-ui/icons";
import { MsgDeleteForm, ActionConfirmation } from "components";
import { useState } from "react";

function MsgDeleteModal(props: { content: any }) {
  const { isOpen, onOpen, onClose } = useDisclosure();
  const {
    isOpen: isOpenAC,
    onOpen: onOpenAC,
    onClose: onCloseAC,
  } = useDisclosure();
  const [erase, setErase] = useState(true);

  function deleteMessage() {
    const queryString =
      "streamName=" +
      props.content["stream"] +
      "&sequenceNumber=" +
      props.content["sequenceNumber"] +
      "&erase=" +
      erase;
    fetch("/api/deleteMessage?" + queryString, {
      method: "DELETE",
    });
  }

  return (
    <>
      <IconButton
        m={2}
        size={"md"}
        onClick={onOpen}
        aria-label="Delete a message"
        icon={<DeleteIcon />}
      />

      <Modal isOpen={isOpen} onClose={onClose} isCentered={true}>
        <ModalOverlay />
        <ModalContent maxW={"600px"}>
          <ModalHeader>Delete message</ModalHeader>
          <ModalCloseButton />
          <ModalBody>
            <MsgDeleteForm
              content={props.content}
              erase={erase}
              setErase={setErase}
            />
          </ModalBody>
          <ModalFooter>
            <Button onClick={onOpenAC} mr={2} colorScheme="red">
              Delete
            </Button>
            <ActionConfirmation
              action={deleteMessage}
              isOpen={isOpenAC}
              onClose={() => {
                onClose();
                onCloseAC();
              }}
              buttonText={"Delete"}
              alertHeader={"Delete Message"}
            />
          </ModalFooter>
        </ModalContent>
      </Modal>
    </>
  );
}

export { MsgDeleteModal };
