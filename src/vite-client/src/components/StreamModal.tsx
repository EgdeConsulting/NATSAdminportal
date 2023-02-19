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
  List,
  ListItem,
  ListIcon,
  OrderedList,
  UnorderedList,
  useStatStyles,
} from "@chakra-ui/react";
import { useState } from "react";

function StreamModal(props: {
  isOpen: boolean;
  onClose: () => void;
  name: string;
}) {
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

  if (props.isOpen) {
    sendStreamName(props.name);
  }

  return (
    <>
      <Modal size={"5xl"} isOpen={props.isOpen} onClose={props.onClose}>
        <ModalOverlay />
        <ModalContent>
          <ModalHeader>Details for "{props.name}"</ModalHeader>
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
