import {
  Box,
  Button,
  Card,
  CardBody,
  Modal,
  ModalBody,
  ModalCloseButton,
  ModalContent,
  ModalFooter,
  ModalHeader,
  ModalOverlay,
  useDisclosure,
} from "@chakra-ui/react";
import { useRef, useEffect, useState } from "react";
import { StreamView } from "components";

function StreamPage() {
  const [streams, setStreams] = useState<[]>([]);

  useEffect(() => {
    getStreams();
  }, [streams.length != 0]);

  function getStreams() {
    fetch("/StreamBasicInfo")
      .then((res) => res.json())
      .then((data) => {
        setStreams(data);
      });
  }

  return (
    <Box>
      <StreamView content={streams} />
    </Box>
  );
}

export { StreamPage };
