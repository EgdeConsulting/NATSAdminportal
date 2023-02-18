import {
  Modal,
  ModalOverlay,
  ModalContent,
  ModalHeader,
  ModalCloseButton,
  ModalBody,
  ModalFooter,
  Button,
  Card,
  CardHeader,
  CardBody,
  useDisclosure,
} from "@chakra-ui/react";
import { useState } from "react";

function StreamModal(props: { content: string }) {
  const { isOpen, onToggle, onClose } = useDisclosure();
  const [streamName, setStreamName] = useState("");
  const [data, setData] = useState({});

  function sendStreamName(name: string) {
    fetch("/api/streamName", {
      method: "POST",
      headers: {
        Accept: "application/json",
        "Content-Type": "application/json",
      },
      body: name,
    })
      .then((res) => res.json())
      .then((jsonData) => {
        console.log(jsonData); //This is a string. WHY
      });
  }

  if (isOpen) {
    sendStreamName(props.content);
  }

  return (
    <>
      <Button
        onClick={() => {
          setStreamName(props.content);
          onToggle();
        }}
        variant={"outline"}
        width={"100%"}
      >
        {props.content}
      </Button>
      <Modal size={"5xl"} isOpen={isOpen} onClose={onClose}>
        <ModalOverlay />
        <ModalContent>
          <ModalHeader>Details for "{props.content}"</ModalHeader>
          <ModalCloseButton />
          <ModalBody>
            <Card>
              <CardHeader>Subjects</CardHeader>
              <CardBody></CardBody>
            </Card>
            <Card>
              <CardHeader>Consumers</CardHeader>
              <CardBody></CardBody>
            </Card>
          </ModalBody>

          <ModalFooter>
            <Button colorScheme="blue" mr={3} onClick={onClose}>
              Close
            </Button>
          </ModalFooter>
        </ModalContent>
      </Modal>
    </>
  );
}

export { StreamModal };
