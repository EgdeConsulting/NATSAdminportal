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
  TooltipHover,
} from "components";
import { useContext, useRef, useState } from "react";

function MsgDeleteModal() {
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
  const finalRef = useRef<HTMLElement>(null);

  function deleteMessage() {
    const msg = currentMsgContext?.currentMsg;
    if (msg) {
      let url = "/api/deleteMessage";

      if (
        !process.env.NODE_ENV ||
        process.env.NODE_ENV === "development+json-server"
      ) {
        url += "/" + msg.sequenceNumber;
      }

      fetch(url, {
        method: "DELETE",
        headers: {
          Accept: "application/json",
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          sequenceNumber: msg.sequenceNumber,
          stream: msg.stream,
          erase: erase,
        }),
      }).then((res) => {
        if (res.ok) {
          changeCurrentMsg(DefaultMsgState.currentMsg);
          changeVisibility(false);
        } else {
          alert(
            "An error occurred while deleting the message: " + res.statusText
          );
        }
      });
    }
  }

  return (
    <>
      <TooltipHover
        label={"Delete message"}
        children={
          <IconButton
            size={"md"}
            aria-label="Delete a message"
            onClick={onOpen}
            icon={<DeleteIcon />}
          />
        }
      />

      <Modal
        finalFocusRef={finalRef}
        isOpen={isOpen}
        onClose={onClose}
        isCentered={true}
      >
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
