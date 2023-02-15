import { Box, Card, CardBody } from "@chakra-ui/react";
import { useRef, useEffect, useState } from "react";
import { StreamView } from "../components";

function StreamPage() {
  const [streams, setStreams] = useState<[]>([]);

  useEffect(() => {
    getStreams();
  }, [streams.length != 0]);

  function getStreams() {
    fetch("/StreamInfo")
      .then((res) => res.json())
      .then((data) => {
        setStreams(data);
        console.log(streams);
      });
  }

  return (
    <Box>
      <StreamView content={streams} />
    </Box>
  );
}

export { StreamPage };
