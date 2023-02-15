import { Button } from "@chakra-ui/react";
import { useRef, useEffect, useState } from "react";

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
      });
  }

  return <Button></Button>;
}

export { StreamPage };
