import {
  Modal,
  ModalOverlay,
  ModalContent,
  ModalHeader,
  ModalCloseButton,
  ModalBody,
  ModalFooter,
  Button,
} from "@chakra-ui/react";

function StreamModal(props: {
  isOpen: boolean;
  onClose: () => void;
  name: string;
}) {
  function sendStreamName(name: string) {
    fetch("/StreamName", {
      method: "POST",
      headers: {
        Accept: "application/json",
        "Content-Type": "application/json",
      },
      body: name,
    }).then((res) => {
      console.log(res.statusText);
      res.json().then((data) => {
        console.log(data);
      });
    });
  }

  if (props.isOpen) {
    sendStreamName(props.name);
  }

  return (
    <>
      <Modal isOpen={props.isOpen} onClose={props.onClose}>
        <ModalOverlay />
        <ModalContent>
          <ModalHeader>Stream Information</ModalHeader>
          <ModalCloseButton />
          <ModalBody>{props.name}</ModalBody>

          <ModalFooter>
            <Button colorScheme="blue" mr={3} onClick={props.onClose}>
              Close
            </Button>
          </ModalFooter>
        </ModalContent>
      </Modal>
    </>
  );
}

export { StreamModal };
