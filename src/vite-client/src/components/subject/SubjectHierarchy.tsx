import { Checkbox, Card, CardHeader, Heading, VStack } from "@chakra-ui/react";
import { useState, useEffect, memo } from "react";
import { LoadingSpinner } from "components";

function SubjectHierarchy() {
  const [subjects, setSubjects] = useState<[]>([]);
  const [loading, setLoading] = useState(true);

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

  const HierarchyCheckbox = memo(
    ({ parent, padding }: { parent: any; padding: number }): JSX.Element => {
      return (
        <>
          <Checkbox m={1} pl={padding} defaultChecked>
            {parent.name}
          </Checkbox>
          {parent.subSubjects != undefined &&
            parent.subSubjects.map((child: any) => {
              return (
                <HierarchyCheckbox
                  key={parent.name + child.name}
                  parent={child}
                  padding={padding + 5}
                />
              );
            })}
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
        <Card variant={"outline"} w={"100%"} mt={"0 !important"}>
          <CardHeader>
            <Heading size={"md"}>Subject Hierarchy</Heading>
          </CardHeader>

          {subjects.map((subject: any, index: number) => {
            return (
              <VStack key={index} ml={4} mb={3} alignItems={"left"}>
                <HierarchyCheckbox parent={subject} padding={0} />
              </VStack>
            );
          })}
        </Card>
      )}
    </>
  );
}

export { SubjectHierarchy };
