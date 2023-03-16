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
import {
  MsgDeleteForm,
  MsgContext,
  ActionConfirmation,
  MsgContextType,
  MsgViewContext,
  DefaultMsgState,
} from "components";
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
  const { changeCurrentMsg } = useContext(MsgContext) as MsgContextType;
  const { changeVisibility } = useContext(MsgViewContext);

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
      }).then((res) => {
        if (res.ok) {
          changeCurrentMsg(DefaultMsgState.currentMsg);
          changeVisibility(false);
        } else {
          alert("There was an error deleting the message.");
        }
      });
    }
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
            <MsgDeleteForm erase={erase} setErase={setErase} />
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
            />
          </ModalFooter>
        </ModalContent>
      </Modal>
    </>
  );
}

export { MsgDeleteModal };
