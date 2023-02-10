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
import { ChatIcon } from "@chakra-ui/icons";
import { ModalForm } from "./";

function MessageModal() {
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
          <ModalHeader>Message Content</ModalHeader>
          <ModalCloseButton />
          <ModalBody>
            <ModalForm />
          </ModalBody>
          <ModalFooter>
            <Button colorScheme="blue">Publish</Button>
            <Button variant="ghost" mr={3} onClick={onClose}>
              Close
            </Button>
          </ModalFooter>
        </ModalContent>
      </Modal>
    </>
  );
}

export { MessageModal };
