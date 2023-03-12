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
import { MsgDeleteForm, MsgContext, ActionConfirmation } from "components";
import { useContext, useState } from "react";

function MsgDeleteModal() {
  const [buttonDisable, toggleButtonDisable] = useState<boolean>(true);
  const { isOpen, onOpen, onClose } = useDisclosure();
  const {
    isOpen: isOpenAC,
    onOpen: onOpenAC,
    onClose: onCloseAC,
  } = useDisclosure();
  const [erase, setErase] = useState(true);
  const currentMsgContext = useContext(MsgContext);

  function deleteMessage() {
    const msg = currentMsgContext?.currentMsg;
    if (msg) {
      const queryString =
        "streamName=" +
        msg.stream +
        "&sequenceNumber=" +
        msg.sequenceNumber +
        "&erase=" +
        erase;
      fetch("/api/deleteMessage?" + queryString, {
        method: "DELETE",
      });
    }
  }

  return (
    <>
      <IconButton
        margin={2}
        size={"md"}
        onClick={onOpen}
        aria-label="Delete a message"
        icon={<DeleteIcon />}
      />

      <Modal isOpen={isOpen} onClose={onClose} isCentered={true}>
        <ModalOverlay />
        <ModalContent maxWidth={"600px"}>
          <ModalHeader>Delete message</ModalHeader>
          <ModalCloseButton />
          <ModalBody>
            <MsgDeleteForm erase={erase} setErase={setErase} />
          </ModalBody>
          <ModalFooter>
            <Button onClick={onOpenAC} marginRight={2} colorScheme="red">
              Delete
            </Button>
            <ActionConfirmation
              action={deleteMessage}
              buttonDisable={buttonDisable}
              toggleButtonDisable={toggleButtonDisable}
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
