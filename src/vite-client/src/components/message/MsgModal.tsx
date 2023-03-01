import {
  Button,
  Modal,
  ModalOverlay,
  ModalContent,
  ModalHeader,
  ModalBody,
  ModalFooter,
  ModalCloseButton,
  useDisclosure,
  Heading,
  Box,
  Stack,
  StackDivider,
  Text,
} from "@chakra-ui/react";
import { useState } from "react";

function MsgModal(props: { content: any }) {
  const { isOpen, onOpen, onClose } = useDisclosure();
  const [messageData, setMessageData] = useState<any>([]);

  function getMessageData() {
    const queryString =
      "stream=" +
      props.content["stream"] +
      "&sequenceNumber=" +
      props.content["sequenceNumber"];
    fetch("/api/messageData?" + queryString)
      .then((res) => res.json())
      .then((data) => {
        setMessageData(data[0]);
      });
  }

  return (
    <>
      <Button
        onClick={() => {
          getMessageData();
          onOpen();
        }}
      >
        View Data
      </Button>

      <Modal isOpen={isOpen} onClose={onClose}>
        <ModalOverlay />
        <ModalContent>
          <ModalHeader>Message Data</ModalHeader>
          <ModalCloseButton />
          <ModalBody>
            <Stack divider={<StackDivider />} spacing={4}>
              <Box>
                <Heading size={"sm"} marginBottom={2}>
                  Headers
                </Heading>
                {messageData["headers"] != undefined ? (
                  <Text>{messageData["headers"]}</Text>
                ) : (
                  <Text>No headers...</Text>
                )}
              </Box>
              <Box>
                <Heading size={"sm"} marginBottom={2}>
                  Payload
                </Heading>
                <Text>{messageData["payload"]}</Text>
              </Box>
            </Stack>
          </ModalBody>
          <ModalFooter>
            <Button variant="ghost" mb={2} mr={3} onClick={onClose}>
              Close
            </Button>
          </ModalFooter>
        </ModalContent>
      </Modal>
    </>
  );
}

export { MsgModal };
