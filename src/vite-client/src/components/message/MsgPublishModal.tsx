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
} from "@chakra-ui/react";
import { ChatIcon } from "@chakra-ui/icons";
import { MsgPublishForm } from "components";

function MsgPublishModal() {
  const { isOpen, onOpen, onClose } = useDisclosure();

  return (
    <>
      <IconButton
        margin={2}
        size={"md"}
        onClick={onOpen}
        aria-label="Publish a message"
        icon={<ChatIcon />}
      />

      <Modal isOpen={isOpen} onClose={onClose}>
        <ModalOverlay />
        <ModalContent>
          <ModalHeader>Publish message</ModalHeader>
          <ModalCloseButton />
          <ModalBody>
            <MsgPublishForm />
            <Button variant="ghost" mb={2} mr={3} onClick={onClose}>
              Close
            </Button>
          </ModalBody>
        </ModalContent>
      </Modal>
    </>
  );
}

export { MsgPublishModal };