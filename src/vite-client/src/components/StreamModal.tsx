import {
  Modal,
  ModalOverlay,
  ModalContent,
  ModalHeader,
  ModalCloseButton,
  ModalBody,
  ModalFooter,
  Button,
  useDisclosure,
  Heading,
  Box,
  Stack,
  StackDivider,
} from "@chakra-ui/react";
import { useState, useEffect } from "react";

function StreamModal(props: { content: string }) {
  const { isOpen, onToggle, onClose } = useDisclosure();
  const [streamData, setStreamData] = useState<any>([]);

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
        // jsonData is a string for some reason
        setStreamData(JSON.parse(jsonData));
      });
  }

  useEffect(() => {
    sendStreamName(props.content);
  }, [isOpen]);

  return (
    <>
      <Button
        onClick={() => {
          onToggle();
        }}
        variant={"outline"}
        width={"100%"}
      >
        {props.content}
      </Button>
      {streamData[0] ? (
        <Modal isOpen={isOpen} onClose={onClose}>
          <ModalOverlay />
          <ModalContent>
            <ModalHeader>
              <Heading size={"md"}>Details for "{props.content}"</Heading>
            </ModalHeader>
            <ModalCloseButton />
            <ModalBody>
              <Stack divider={<StackDivider />} spacing={4}>
                <Box>
                  <Heading size={"sm"} marginBottom={2}>
                    Description
                  </Heading>

                  {streamData[0]["Description"].length != 0 ? (
                    streamData[0]["Description"]
                  ) : (
                    <div>No description...</div>
                  )}
                </Box>
                <Box>
                  <Heading size={"sm"} marginBottom={2}>
                    Subjects
                  </Heading>

                  {streamData[0]["Subjects"].map(
                    (subject: string, key: number) => {
                      return <div key={key}>{subject}</div>;
                    }
                  )}
                </Box>
                <Box>
                  <Heading size={"sm"} marginBottom={2}>
                    Consumers
                  </Heading>

                  {streamData[0]["Consumers"].length != 0 ? (
                    streamData[0]["Consumers"].map(
                      (consumer: string, key: number) => {
                        return <div key={key}>{consumer}</div>;
                      }
                    )
                  ) : (
                    <div>No consumers...</div>
                  )}
                </Box>
                <Box>
                  <Heading size={"sm"} marginBottom={2}>
                    Policies
                  </Heading>

                  {streamData[0]["Policies"].map((policy: any, key: number) => {
                    return (
                      <div key={key}>
                        {/* Probably a better way to display policy string, but this will do for now */}
                        {JSON.stringify(policy)
                          .replace(/{|}|"/g, "")
                          .replace(":", ":\t")}
                      </div>
                    );
                  })}
                </Box>
                <Box>
                  <Heading size={"sm"} marginBottom={2}>
                    Deleted
                  </Heading>

                  {<div>Deleted messages: {streamData[0]["Deleted"]}</div>}
                </Box>
              </Stack>
            </ModalBody>
            <ModalFooter>
              <Button colorScheme="blue" mr={3} onClick={onClose}>
                Close
              </Button>
            </ModalFooter>
          </ModalContent>
        </Modal>
      ) : null}
    </>
  );
}

export { StreamModal };
