import {
  List,
  ListItem,
  Card,
  CardHeader,
  Heading,
  VStack,
  Accordion,
  AccordionItem,
  AccordionButton,
  AccordionPanel,
  AccordionIcon,
  Divider,
  Button,
  HStack,
} from "@chakra-ui/react";
import { useState, useEffect, memo } from "react";
import { LoadingSpinner } from "components";

function SubjectHierarchy() {
  const [subjects, setSubjects] = useState<ISubject[]>([]);
  const [loading, setLoading] = useState(true);
  const [expanded, toggleExpanded] = useState<boolean>(false);
  interface ISubject {
    name: string;
    subSubjects: ISubject[];
  }

  useEffect(() => {
    getSubjects();
  }, [subjects.length == 0]);

  function getSubjects() {
    fetch("/api/subjectHierarchy")
      .then((res) => res.json())
      .then((data) => {
        setSubjects(data);
        setLoading(false);
      });
  }

  // Alternating background color for each element
  const HierarchyList = memo(
    ({
      parent,
      padding,
    }: {
      parent: ISubject;
      padding: number;
    }): JSX.Element => {
      return (
        <>
          <List spacing={1} pl={padding}>
            <Accordion allowMultiple index={expanded ? 0 : 1}>
              <AccordionItem>
                {parent.subSubjects != undefined ? (
                  <AccordionButton>
                    <ListItem>{parent.name}</ListItem>
                    <AccordionIcon />
                  </AccordionButton>
                ) : (
                  <ListItem>{parent.name}</ListItem>
                )}
                <AccordionPanel pb={2} pt={0}>
                  {parent.subSubjects != undefined &&
                    parent.subSubjects.map((child: ISubject) => {
                      return (
                        <HierarchyList
                          key={parent.name + child.name}
                          parent={child}
                          padding={padding + 5}
                        />
                      );
                    })}
                </AccordionPanel>
              </AccordionItem>
            </Accordion>
          </List>
        </>
      );
    }
  );

  return (
    <>
      {loading ? (
        <Card variant={"outline"} w={"100%"} mt={"0 !important"}>
          <CardHeader>
            <Heading size={"md"}>Subject Hierarchy</Heading>
          </CardHeader>
          <LoadingSpinner />
        </Card>
      ) : (
        <Card variant={"outline"} w={"100%"} mt={"0 !important"} pb={4}>
          <CardHeader>
            <HStack spacing={"auto"}>
              <Heading size={"md"}>Subject Hierarchy</Heading>
              <Button
                onClick={() =>
                  expanded ? toggleExpanded(false) : toggleExpanded(true)
                }
              >
                {expanded ? "Collapse all" : "Expand all"}
              </Button>
            </HStack>

            <Divider w={"100%"} mt={2} />
          </CardHeader>

          {subjects.map((subject: ISubject, index: number) => {
            return (
              <VStack key={index} mx={4} alignItems={"left"}>
                <HierarchyList parent={subject} padding={0} />
              </VStack>
            );
          })}
        </Card>
      )}
    </>
  );
}

export { SubjectHierarchy };
