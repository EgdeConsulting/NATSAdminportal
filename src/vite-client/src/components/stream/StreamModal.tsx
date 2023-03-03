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
  Text,
} from "@chakra-ui/react";
import { useState, useEffect } from "react";

function StreamModal(props: { content: string }) {
  const { isOpen, onToggle, onClose } = useDisclosure();
  const [streamData, setStreamData] = useState<any>([]);

  function sendStreamName(name: string) {
    const queryString = "streamName=" + name;
    fetch("/api/streamName?" + queryString)
      .then((res) => res.json())
      .then((data) => {
        setStreamData(data);
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
      {streamData ? (
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

                  {streamData.description != undefined &&
                  streamData.description.length != 0 ? (
                    streamData.desciption
                  ) : (
                    <div>No description...</div>
                  )}
                </Box>
                <Box>
                  <Heading size={"sm"} marginBottom={2}>
                    Subjects
                  </Heading>
                  {streamData.subjects != undefined &&
                    streamData.subjects.map((subject: string, key: number) => {
                      return <div key={key}>{subject}</div>;
                    })}
                </Box>
                <Box>
                  <Heading size={"sm"} marginBottom={2}>
                    Consumers
                  </Heading>

                  {streamData.consumers != undefined &&
                  streamData.consumers.length != 0 ? (
                    streamData.consumers.map(
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
                  {streamData.policies != undefined &&
                  Object.entries(streamData.policies).length != 0 ? (
                    Object.entries(streamData.policies).map(
                      ([key, value], index: number) => (
                        <Text key={index} fontSize={"md"}>
                          {key + " : " + value}
                        </Text>
                      )
                    )
                  ) : (
                    <Text fontSize={"md"}>No Policies...</Text>
                  )}
                  {/* {streamData.policies.map((policy: any, key: number) => {
                    return (
                      <div key={key}>
                        
                        {JSON.stringify(policy)
                          .replace(/{|}|"/g, "")
                          .replace(":", ":\t")}
                      </div>
                    );
                  })} */}
                </Box>
                <Box>
                  <Heading size={"sm"} marginBottom={2}>
                    Deleted
                  </Heading>

                  {
                    <div>
                      Deleted messages:{" "}
                      {streamData.deleted != undefined && streamData.deleted}
                    </div>
                  }
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
