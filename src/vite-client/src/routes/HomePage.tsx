import { Flex, Card, CardBody } from "@chakra-ui/react";
import { PageHeader } from "components";

function HomePage() {
  return (
    <Flex w={"100%"}>
      <Card variant={"outline"} w={"100%"} mt={2} mr={2}>
        <CardBody>
          <PageHeader
            heading={"Welcome to Egdes NATS-administrationportal"}
            introduction={"General info here..."}
          />
        </CardBody>
      </Card>
    </Flex>
  );
}

export { HomePage };
